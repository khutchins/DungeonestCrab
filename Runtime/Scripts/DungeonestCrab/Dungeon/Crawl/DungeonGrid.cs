using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ratferences;
using KH.Extensions;
using KH.Graph;
using System.Linq;
using KH;
using DungeonestCrab.Dungeon.Pen;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DungeonestCrab.Dungeon.Crawl {
    [DefaultExecutionOrder(-100)]
    public class DungeonGrid : MonoBehaviour {

        [SerializeField] BoolReference MovementQueueingAllowed;
        [SerializeField] TaskQueue TaskQueue;

        public interface Resettable {
            public void ResetEntity();
        }

        public interface GridObject {

            /// <summary>
            /// Info about the object's current position on the grid.
            /// Normally should just be pulled from the DungeonGrid using
            /// its transform's position.
            /// </summary>
            public GridItemInfo GridItemInfo();
        }

        private static DungeonGrid _instance;
        private DungeonMover _player;
        protected List<DungeonEntrance> _entrances = new();
        private Dictionary<Vector2Int, Node> _nodes = new Dictionary<Vector2Int, Node>();
        private Dictionary<DungeonInteractable, Node> _interactableMap = new Dictionary<DungeonInteractable, Node>();
        private Dictionary<DungeonInteractable, Node.Edge> _interactableMapEdge = new Dictionary<DungeonInteractable, Node.Edge>();
        private HashSet<DungeonEntity> _registeredEntities = new HashSet<DungeonEntity>();
        private HashSet<DungeonEntity> _unregisteredEntities = new HashSet<DungeonEntity>();
        private List<Node> _allNodes = new List<Node>();
        private Vector2Int _minBounds;
        private Vector2Int _maxBounds;
        private bool _movementAllowed = true;
        private Dictionary<Node, DirectedGraph<Node>.Node> _graphMap = new Dictionary<Node, KH.Graph.DirectedGraph<Node>.Node>();
        private DirectedGraph<Node> _graph = new DirectedGraph<Node>();

        public bool MovementAllowed {
            get => _movementAllowed;
        }

        public delegate void OnNodeChanged(Node node);
        public OnNodeChanged NodeChanged;

        [SerializeField] private Vector2 _cellSize = Vector2.one;

        public Vector2 CellSize {
            get => new Vector2(_cellSize.x, _cellSize.y);
        }

        public Vector2Int MinBounds { get => _minBounds; }
        public Vector2Int MaxBounds { get => _maxBounds; }
        public Vector2Int Size { get => _maxBounds - _minBounds + Vector2Int.one; }
        public List<Node> AllNodes { get => _allNodes; }

        public static DungeonGrid INSTANCE {
            get {
                if (_instance != null) return _instance;
#if UNITY_EDITOR
                _instance = GameObject.FindObjectOfType<DungeonGrid>();

#endif
                if (_instance == null) {
                    Debug.LogWarning("No instance of DungeonGrid exists!");
                }
                return _instance;
            }
        }

        public struct GridItemInfo {
            public Vector2 GridPosition;
            public Vector3 WorldPosition;
            public bool OnEdge;

            public GridItemInfo(Vector2 gridPosition, Vector3 worldPosition, bool onEdge) {
                GridPosition = gridPosition;
                WorldPosition = worldPosition;
                OnEdge = onEdge;
            }
        }

        public GridItemInfo InfoForGridPosition(Vector2 loc) {
            return InfoForLocation(loc * _cellSize);
        }

        public GridItemInfo InfoForLocation(Vector2 loc) {
            return InfoForLocation(new Vector3(loc.x, 0, loc.y));
        }

        public GridItemInfo InfoForLocation(Vector3 loc) {
            Vector2 vec2Loc = new Vector2(loc.x, loc.z);
            Vector2 gridPos = vec2Loc / _cellSize;

            bool xEdge = IsOnEdge(gridPos.x);
            bool yEdge = IsOnEdge(gridPos.y);

            Vector2 canonicalPos = UniformGridPos(gridPos, xEdge || yEdge);
            Vector3 worldPos = new Vector3(canonicalPos.x * _cellSize.x, loc.y, canonicalPos.y * _cellSize.y);

            GridItemInfo info = new GridItemInfo(canonicalPos, worldPos, xEdge || yEdge);
            return info;
        }

        private Vector2 UniformGridPos(Vector2 gridSpacePos, bool onEdge) {
            if (!onEdge) {
                return Vector2Int.RoundToInt(gridSpacePos);
            }

            Vector2Int floor = Floor(gridSpacePos);
            Vector2Int ceil = Ceil(gridSpacePos);

            return new Vector2(floor.x + ceil.x, floor.y + ceil.y) / 2f;
        }

        private void Awake() {
            _instance = this;
            _minBounds = new Vector2Int(int.MaxValue, int.MaxValue);
            _maxBounds = new Vector2Int(int.MinValue, int.MinValue);
        }

        public void RegisterPlayer(DungeonMover mover) {
            _player = mover;
        }

        public void RegisterEntrance(DungeonEntrance entrance) {
            _entrances.Add(entrance);
        }

        public void RegisterNode(Vector2 pos, bool walkable = true) {
            Vector2Int normalizedPos = RoundedNode(pos);

            GridItemInfo info = InfoForGridPosition(normalizedPos);
            Node node = new Node(info.WorldPosition, normalizedPos, walkable);
            if (_nodes.ContainsKey(normalizedPos)) {
                Debug.LogWarning($"Cell already exists at {pos}. Ignoring this.");
                return;
            }
            _nodes.Add(normalizedPos, node);
            _allNodes.Add(node);
            _minBounds.x = Math.Min(normalizedPos.x, _minBounds.x);
            _maxBounds.x = Math.Max(normalizedPos.x, _maxBounds.x);
            _minBounds.y = Math.Min(normalizedPos.y, _minBounds.y);
            _maxBounds.y = Math.Max(normalizedPos.y, _maxBounds.y);
            _graphMap[node] = _graph.AddNode(node);
            TryConnectNode(node);
        }

        public void MapFinishedLoading() {
            PositionPlayer();
        }

        protected virtual DungeonEntrance GetEntrance() {
            if (_entrances.Count <= 0) {
                Debug.LogError("No entrance registered. This will not work.");
                return null;
            }
            return _entrances[0];
        }

        void PositionPlayer() {
            DungeonEntrance entrance = GetEntrance();
            _player.TeleportToGridObject(entrance, entrance.Angle);
        }

        public void SaveAndRespawn() {
            List<DungeonEntity> entitiesToReset = _registeredEntities.Concat(_unregisteredEntities).ToList();
            foreach (var entity in entitiesToReset) {
                entity.ResetEntity();
            }
            if (_player != null) _player.ResetEntity();
        }

        public void RegisterNode(GridObject obj, bool walkable = true) {
            GridItemInfo info = obj.GridItemInfo();
            RegisterNode(info.GridPosition, walkable);
        }

        public bool IsOnEdge(Vector3 loc) {
            Vector2 vec2Loc = new Vector2(loc.x, loc.z);
            Vector2 mod = vec2Loc / _cellSize;
            return IsOnEdge(mod.x) || IsOnEdge(mod.y);
        }

        private bool IsOnEdge(float num) {
            return Mathf.Abs(0.5f - Mathf.Abs(num % 1)) < 0.1f;
        }

        public void UpdateEntity(DungeonInteractable interactable) {
            UpdateNodesForEntity(interactable);
        }

        public void RegisterEntity(DungeonInteractable interactable, GridItemInfo info) {
            if (info.OnEdge) {
                RegisterEntity(interactable, Floor(info.GridPosition), Ceil(info.GridPosition));
            } else {
                RegisterEntity(interactable, info.GridPosition);
            }
        }

        public void RegisterEntity(DungeonInteractable interactable, Vector2 pos) {
            Vector2Int normalizedPos = RoundedNode(pos);

            RemoveInteractable(interactable);
            _registeredEntities.Add(interactable);

            Node node = TryGetNode(normalizedPos);
            node?.MaybeAddInteractable(interactable);
            _interactableMap[interactable] = node;
            if (node != null) NodeChanged?.Invoke(node);
        }

        public void RegisterEntity(DungeonInteractable interactable, Vector2Int pos1, Vector2Int pos2) {
            RemoveInteractable(interactable);
            _registeredEntities.Add(interactable);

            Node node1 = TryGetNode(pos1);
            Node node2 = TryGetNode(pos2);

            if (node1 == null) {
                RegisterNode(pos1, false);
                node1 = TryGetNode(pos1);
            }
            if (node2 == null) {
                RegisterNode(pos2, false);
                node2 = TryGetNode(pos2);
            }

            Node.Edge edge = node1.EdgeForNeighbor(node2);
            edge?.MaybeAddInteractable(interactable);
            _interactableMapEdge[interactable] = edge;
            if (node1 != null) NodeChanged?.Invoke(node1);
            if (node2 != null) NodeChanged?.Invoke(node2);
        }

        private void UpdateNodesForEntity(DungeonInteractable interactable) {
            if (_interactableMap.TryGetValue(interactable, out Node node)) {
                NodeChanged?.Invoke(node);
            }

            if (_interactableMapEdge.TryGetValue(interactable, out Node.Edge edge)) {
                foreach (Node edgeNode in edge.Nodes) {
                    NodeChanged?.Invoke(edgeNode);
                }
            }
        }

        public void UnregisterEntity(DungeonInteractable interactable) {
            _unregisteredEntities.Add(interactable);
            RemoveInteractable(interactable);
        }

        public Node TryGetNode(Vector2Int pos) {
            if (_nodes.ContainsKey(pos)) {
                return _nodes[pos];
            }
            return null;
        }

        private void RemoveInteractable(DungeonInteractable interactable) {
            _registeredEntities.Remove(interactable);
            if (_interactableMap.ContainsKey(interactable)) {
                Node old = _interactableMap[interactable];
                _interactableMap.Remove(interactable);
                if (old != null) {
                    old.Interactables.Remove(interactable);
                    NodeChanged?.Invoke(old);
                }
            }
            if (_interactableMapEdge.ContainsKey(interactable)) {
                Node.Edge old = _interactableMapEdge[interactable];
                _interactableMapEdge.Remove(interactable);
                if (old != null) {
                    old.Interactables.Remove(interactable);
                    NodeChanged?.Invoke(old.Nodes[0]);
                    NodeChanged?.Invoke(old.Nodes[1]);
                }
            }
        }

        public Vector2 CanonicalOffset(Vector3 dir) {
            return new Vector3(dir.x * _cellSize.x, dir.z * _cellSize.y);
        }

        public bool CanMove(Node.EdgeNode path) {
            return path != null
                && path.Node.Walkable
                && path.Edge.Interactables.TrueForAll(di => !di.BlocksMovement)
                && path.Node.Interactables.TrueForAll(di => !di.BlocksMovement);
        }

        public bool CanMove(Vector2 from, Vector2 dir) {
            Vector2Int curr = RoundedNode(from);
            Node.EdgeNode to = GetNodeInDir(curr, dir);
            return CanMove(to);
        }

        public DungeonInteractable InteractableForMove(Vector2 from, Vector2 dir) {
            Vector2Int curr = RoundedNode(from);
            Node.EdgeNode to = GetNodeInDir(curr, dir);
            if (to == null) return null;
            if (to.Edge.Interactables.Count > 0) return to.Edge.Interactables[0];
            if (to.Node.Interactables.Count > 0) return to.Node.Interactables[0];
            return null;
        }

        Vector2Int RoundedNode(Vector2 pos) {
            return Vector2Int.RoundToInt(pos);
        }


        public Node.EdgeNode GetNodeInDir(Vector2Int pos, Vector2 offset) {
            if (!_nodes.ContainsKey(pos)) {
                return null;
            }
            Node node = _nodes[pos];
            return node.Neighbor(offset);
        }

        private void TryConnectNode(Node node) {
            TryConnectNode(node, Node.Dir.North);
            TryConnectNode(node, Node.Dir.East);
            TryConnectNode(node, Node.Dir.South);
            TryConnectNode(node, Node.Dir.West);
        }

        private void TryConnectNode(Node node, Node.Dir dir) {
            Node neighbor = GetNodeInDir(node.Coords, dir);
            if (neighbor == null) return;
            Connect(neighbor, node);
        }

        private void Connect(Node node1, Node node2) {
            Node.Edge edge = node1.AddNeighbor(node2);
            node2.AddNeighbor(node1, edge);

            DirectedGraph<Node>.Node n1 = _graphMap[node1];
            DirectedGraph<Node>.Node n2 = _graphMap[node2];
            _graph.AddEdge(n1, n2, 1);
            _graph.AddEdge(n2, n1, 1);
        }

        Node GetNodeInDir(Vector2Int pos, Node.Dir dir) {
            var offset = dir switch {
                Node.Dir.East => Vector2Int.right,
                Node.Dir.South => Vector2Int.left,
                Node.Dir.West => Vector2Int.down,
                _ => Vector2Int.up,
            };
            Vector2Int adjustedPos = pos + offset;
            if (_nodes.ContainsKey(adjustedPos)) {
                return _nodes[adjustedPos];
            }
            return null;
        }

        private Vector2Int Floor(Vector2 val) {
            return new Vector2Int(ProbablyFloor(val.x), ProbablyFloor(val.y));
        }
        private Vector2Int Ceil(Vector2 val) {
            return new Vector2Int(ProbablyCeil(val.x), ProbablyCeil(val.y));
        }

        private int ProbablyFloor(float val) {
            // Can't use Vector2.FloorToInt because floats have a nasty habit of being just under
            // the integer value that they're set to.
            int rounded = Mathf.RoundToInt(val);
            return (Mathf.Abs(rounded - val) < 0.05f) ? rounded : Mathf.FloorToInt(val);
        }

        private int ProbablyCeil(float val) {
            // Can't use Vector2.CeilToInt because floats have a nasty habit of being just above
            // the integer value that they're set to.
            int rounded = Mathf.RoundToInt(val);
            return (Mathf.Abs(rounded - val) < 0.05f) ? rounded : Mathf.CeilToInt(val);
        }

        #region MOVEMENT COORDINATION

        private struct DesiredMove {
            public readonly DungeonEntity entity;
            public readonly DungeonEntity.TurnAction turn;
            public readonly Node from;
            public readonly Node.EdgeNode edgeNode;
            public readonly Node.Edge edge;
            public readonly Node to;
            public MoveAttempt attempt;

            public DesiredMove(DungeonEntity entity, DungeonEntity.TurnAction turn, Node from, Node.EdgeNode edgeNode) {
                this.entity = entity;
                this.turn = turn;
                this.from = from;
                this.edge = null;
                this.to = null;
                this.edgeNode = edgeNode;
                if (edgeNode != null) {
                    this.edge = edgeNode.Edge;
                    this.to = edgeNode.Node;
                } else {
                    // If we don't have a move, set the end node anyway.
                    // We need it for detecting collisions.
                    this.to = from;
                }
                this.attempt = null;
            }

            public DesiredMove(DungeonEntity entity, Node.Edge edge) {
                this.entity = entity;
                this.turn = DungeonEntity.TurnAction.DoNothing;
                this.from = null;
                this.edgeNode = null;
                this.edge = edge;
                this.to = null;
                this.attempt = null;
            }

            public bool ShouldDo {
                get => turn != DungeonEntity.TurnAction.DoNothing;
            }

            public bool Collides(DesiredMove other) {
                return other.edge == this.edge || other.to == this.to;
            }
        }

        public class MoveAttempt {
            public Vector3 from;
            public Vector3 to;
            public bool isBump;
            public bool isWallBump;
        }

        private DesiredMove GetDesiredMove(DungeonEntity entity, DungeonEntity.TurnAction action) {
            GridItemInfo info = entity.GridItemInfo();
            Vector2Int offset = entity.RealMoveDir(action);


            if (info.OnEdge) {
                Node.Edge edge = EdgeForInfo(info);
                return new DesiredMove(entity, edge);
            } else {
                Vector2Int curr = RoundedNode(info.GridPosition);
                Node from = TryGetNode(curr);
                Node.EdgeNode edge = from != null && offset.sqrMagnitude > 0 ? from.Neighbor(offset) : null;
                return new DesiredMove(entity, action, from, edge);
            }
        }

        private Node.Edge EdgeForInfo(GridItemInfo info) {
            Vector2Int pos1 = Floor(info.GridPosition);
            Vector2Int pos2 = Ceil(info.GridPosition);

            Node node1 = TryGetNode(pos1);
            Node node2 = TryGetNode(pos2);
            Node.Edge edge = node1.EdgeForNeighbor(node2);
            return edge;
        }

        private MoveAttempt GetMoveAttempt(DesiredMove move) {
            GridItemInfo info = move.entity.GridItemInfo();
            Vector2Int offset = move.entity.RealMoveDir(move.turn);

            Vector2Int curr = RoundedNode(info.GridPosition);

            Node.EdgeNode to = offset.sqrMagnitude > 0 ? GetNodeInDir(curr, offset) : null;

            bool walkable = to != null && to.Node.Walkable;

            MoveAttempt attempt = new MoveAttempt {
                from = info.WorldPosition,
                to = info.WorldPosition + (offset * _cellSize).ToVectorX0Y(),
                isBump = !walkable,
                isWallBump = true,
            };
            return attempt;
        }

        private MoveAttempt GetMoveAttemptSingleMove(DesiredMove move) {
            GridItemInfo info = move.entity.GridItemInfo();
            Vector2Int offset = move.entity.RealMoveDir(move.turn);

            Vector2Int curr = RoundedNode(info.GridPosition);

            Node.EdgeNode to = offset.sqrMagnitude > 0 ? GetNodeInDir(curr, offset) : null;

            bool walkable = to != null && to.Node.Walkable;
            bool interactableBlockingPath = walkable && !CanMove(move.edgeNode);

            MoveAttempt attempt = new MoveAttempt {
                from = info.WorldPosition,
                to = info.WorldPosition + (offset * _cellSize).ToVectorX0Y(),
                isBump = !walkable || interactableBlockingPath,
                isWallBump = !interactableBlockingPath,
            };
            return attempt;
        }

        public static DungeonInteractable InteractableForMove(Node.EdgeNode to, bool manualInteraction) {
            if (to == null) return null;
            DungeonInteractable interact = to.Edge.Interactables.Where(x => !manualInteraction || !x.IgnoreInteractButton).FirstOrDefault();
            if (interact != null) return interact;
            return to.Node.Interactables.Where(x => !manualInteraction || !x.IgnoreInteractButton).FirstOrDefault();
        }

        private void MaybeAdd(Dictionary<object, List<DesiredMove>> dict, object key, DesiredMove move) {
            if (key == null) return;
            if (!dict.TryGetValue(key, out List<DesiredMove> list)) {
                list = new List<DesiredMove>();
                dict[key] = list;
            }
            list.Add(move);
        }

        private void AddAppropriate(Dictionary<object, List<DesiredMove>> dict, DesiredMove move) {
            //MaybeAdd(dict, move.from, move);
            MaybeAdd(dict, move.edge, move);
            MaybeAdd(dict, move.to, move);
        }

        public bool CanDoMove(DungeonEntity entity, DungeonEntity.TurnAction action) {
            DesiredMove move = GetDesiredMove(entity, action);
            return CanMove(move.edgeNode);
        }

        /// <summary>
        /// Computes the turn action to hunt the player, with do nothing returned
        /// if no player is found.
        /// </summary>
        /// <param name="entity">The player hunting</param>
        /// <returns></returns>
        public DungeonEntity.TurnAction HuntPlayer(DungeonInteractable entity) {
            GridItemInfo pInfo = _player.GridItemInfo();
            return MoveForPathToNode(entity, pInfo.GridPosition);
        }

        public GridItemInfo PlayerPosition => _player.GridItemInfo();

        public DungeonEntity.TurnAction MoveForPathToNode(DungeonInteractable entity, Vector2 gridPosition) {
            Node node = TryGetNode(RoundedNode(entity.GridItemInfo().GridPosition));
            Node pNode = TryGetNode(RoundedNode(gridPosition));
            if (node == null || !_graphMap.ContainsKey(node) || pNode == null || !_graphMap.ContainsKey(pNode)) {
                Debug.LogWarning($"Failed to find node info {node} {pNode}");
            }

            var path = _graph.FindPath(_graphMap[node], _graphMap[pNode], (Node n1, Node n2) => {
                return Mathf.Abs(n1.Coords.x - n2.Coords.x) + Mathf.Abs(n1.Coords.y - n2.Coords.y);
            });

            if (path == null || path.Count == 0) return DungeonEntity.TurnAction.DoNothing;

            var first = path[0];
            Node second = first.Element;
            return ActionForOffset(second.Coords - node.Coords);
        }

        public DungeonEntity.TurnAction ActionForOffset(Vector2 offset) {
            if (offset.sqrMagnitude < 0.1f) return DungeonEntity.TurnAction.DoNothing;
            if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y)) {
                if (offset.x > 0) return DungeonEntity.TurnAction.MoveEast;
                else return DungeonEntity.TurnAction.MoveWest;
            } else {
                if (offset.y > 0) return DungeonEntity.TurnAction.MoveNorth;
                else return DungeonEntity.TurnAction.MoveSouth;
            }
        }

        public void HandleInteractions(DungeonMover player) {
            if (!_movementAllowed) {
                Debug.LogWarning("Tried to interact while moving!");
            }
            _movementAllowed = false;

            DesiredMove pMove = GetDesiredMove(player, DungeonEntity.TurnAction.MoveForward);
            DungeonInteractable interact = InteractableForMove(pMove.edgeNode, true);
            StartCoroutine(IndependentInteractCoroutine(player, interact));
        }

        IEnumerator IndependentInteractCoroutine(DungeonMover mover, params DungeonInteractable[] interacts) {
            yield return HandleInteracts(mover, interacts);
            _movementAllowed = true;

            if (mover) mover.OnReadyForInput();
        }

        public void InitiateSingleMove(DungeonEntity entity, DungeonEntity.TurnAction action, DungeonMover.IMovementConfig movementConfig, bool interactOnBump) {
            StartCoroutine(DoSingleMove(entity, action, movementConfig, interactOnBump));
        }

        public IEnumerator DoSingleMove(DungeonEntity entity, DungeonEntity.TurnAction action, DungeonMover.IMovementConfig movementConfig, bool interactOnBump) {
            DesiredMove move = GetDesiredMove(entity, action);
            move.attempt = GetMoveAttemptSingleMove(move);

            yield return move.entity.DoTurnAction(move.turn, move.attempt, movementConfig);
            DungeonInteractable interact = InteractableForMove(move.edgeNode, false);
            if (interact != null && move.entity is DungeonMover) {
                yield return (move.entity as DungeonMover).HandleInteract(interact);
            }
            UpdateEntityOnGrid(move.entity);
        }

        public MoveAttempt GetMoveAttempt(DungeonEntity entity, DungeonEntity.TurnAction action) {
            DesiredMove move = GetDesiredMove(entity, action);
            return GetMoveAttemptSingleMove(move);
        }

        private IEnumerator WaitCoroutine(float delay) {
            yield return new WaitForSeconds(delay);
        }

        private void UpdateEntityOnGrid(DungeonEntity entity) {
            if (entity is DungeonInteractable interact) {
                DungeonGrid.GridItemInfo info = interact.GridItemInfo();
                DungeonGrid.INSTANCE.RegisterEntity(interact, info);
            }
        }

        private IEnumerator HandleInteractsEnumerable(DungeonMover mover, IEnumerable<DungeonInteractable> interacts) {
            // disable movement queueing
            MovementQueueingAllowed.Value = false;

            // handle player interactions one by one
            foreach (DungeonInteractable interact in interacts) {
                yield return mover.HandleInteract(interact);
            }

            // enable movement queueing
            MovementQueueingAllowed.Value = true;
        }

        private IEnumerator HandleInteracts(DungeonMover mover, params DungeonInteractable[] interacts) {
            yield return HandleInteractsEnumerable(mover, interacts);
        }

        static bool ShouldShowEntityOnMap(IEnumerable<DungeonInteractable> interacts) {
            foreach (DungeonInteractable interact in interacts) {
                if (interact.ShowOnMap) return true;
            }
            return false;
        }

        #endregion

        public class Node {

            public enum Dir {
                North = 0,
                East = 1,
                South = 2,
                West = 3
            }

            public readonly Vector2Int Coords;
            public readonly Vector3 WorldPos;
            /// <summary>
            /// Whether a node or not is walkable. Generally not needed, but used for when an interactable is on an edge betwee two nodes, but only one exists on the grid.
            /// </summary>
            public bool Walkable;
            EdgeNode[] Neighbors = new EdgeNode[4];
            public List<DungeonInteractable> Interactables = new List<DungeonInteractable>();

            public Node(Vector3 worldPos, Vector2Int coords, bool walkable = true) {
                Coords = coords;
                WorldPos = worldPos;
                Walkable = walkable;
            }

            public bool ShowEntityOnMap() {
                return ShouldShowEntityOnMap(Interactables);
            }

            public Edge AddNeighbor(Node node) {
                int offset = (int)DirForOffset(node.Coords - Coords);
                if (Neighbors[offset] != null) {
                    Debug.LogWarning($"Attempting to connect connected node {Coords} - {node.Coords}. This is probably bad!");
                }
                Edge edge = new Edge(this, node);
                Neighbors[offset] = new EdgeNode(node, edge);
                return edge;
            }

            public void AddNeighbor(Node node, Edge edge) {
                Neighbors[(int)DirForOffset(node.Coords - Coords)] = new EdgeNode(node, edge);
            }

            public EdgeNode Neighbor(Vector2 offset) {
                return Neighbor(DirForOffset(offset));
            }

            public EdgeNode Neighbor(Dir dir) {
                return Neighbors[(int)dir];
            }

            public static bool IsTraversable(EdgeNode edgeNode) {
                return edgeNode != null && edgeNode.Node != null && edgeNode.Node.Walkable;
            }

            /// <summary>
            /// Sees if the corner defined by the two directions should be drawn. (i.e. does one of the nodes not exist.)
            /// </summary>
            /// <returns>Whether a corner wall should be drawn..</returns>
            public bool ShouldDrawCornerWall(Dir dir1, Dir dir2) {
                EdgeNode n1 = Neighbor(dir1);
                EdgeNode n2 = Neighbor(dir2);
                if (!IsTraversable(n1) || !IsTraversable(n2)) return true;
                EdgeNode n12 = n1.Node.Neighbor(dir2);
                return !IsTraversable(n12);
            }

            public Edge EdgeForNeighbor(Node node) {
                foreach (EdgeNode edgeNode in Neighbors) {
                    if (edgeNode != null && edgeNode.Node == node) return edgeNode.Edge;
                }
                return null;
            }

            public void MaybeAddInteractable(DungeonInteractable interactable) {
                if (!Interactables.Contains(interactable)) {
                    Interactables.Add(interactable);
                }
            }

            public Dir DirForOffset(Vector2 offset) {
                bool primarilyX = Mathf.Abs(offset.x) > Mathf.Abs(offset.y);
                if (offset.x > 0 && primarilyX) return Dir.East;
                if (offset.x < 0 && primarilyX) return Dir.West;
                if (offset.y > 0) return Dir.North;
                if (offset.y < 0) return Dir.South;
                Debug.LogWarning("Offset was zero");
                return Dir.North;
            }

            public class EdgeNode {
                public readonly Node Node;
                public readonly Edge Edge;

                public EdgeNode(Node node, Edge edge) {
                    Node = node;
                    Edge = edge;
                }
            }

            public class Edge {
                public readonly Node[] Nodes;
                public List<DungeonInteractable> Interactables = new List<DungeonInteractable>();

                public Edge(Node node1, Node node2) {
                    Nodes = new Node[2] { node1, node2 };
                }

                public bool ShowEntityOnMap() {
                    return DungeonGrid.ShouldShowEntityOnMap(Interactables);
                }

                public void MaybeAddInteractable(DungeonInteractable interactable) {
                    if (!Interactables.Contains(interactable)) {
                        Interactables.Add(interactable);
                    }
                }

                public Node Other(Node node) {
                    return Nodes[0] == node ? Nodes[1] : Nodes[0];
                }
            }
        }
    }
}