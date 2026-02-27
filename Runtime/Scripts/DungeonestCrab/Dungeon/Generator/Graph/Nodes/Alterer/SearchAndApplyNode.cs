using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Passes/Logic/Search & Apply")]
    public class SearchAndApplyNode : DungeonPassNode {

        [Header("Inputs")]
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public MatcherConnection SearchCriteria;
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public MatcherConnection TargetCriteria;
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public ActionConnection Action;

        [Header("Settings")]
        [Tooltip("The range around tiles matching the search criteria to look for target tiles.")]
        public int SearchRadius = 0;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            IMatcher searchCriteria = GetInputValue<IMatcher>("SearchCriteria", null);
            if (searchCriteria == null) searchCriteria = TileMatcher.MatchingAll();

            IMatcher targetCriteria = GetInputValue<IMatcher>("TargetCriteria", null);
            ITileAction action = GetInputValue<ITileAction>("Action", null);

            var logic = new SearchAndApply(
                searchCriteria, 
                targetCriteria, 
                SearchRadius,
                action
            );

            return logic.Modify(dungeon, random);
        }
    }
}
