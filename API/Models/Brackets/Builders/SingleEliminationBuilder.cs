namespace API.Models.Brackets.Builders;

public class SingleEliminationBuilder : BracketBuilder
{
    public SingleEliminationBuilder(SnowflakeService snowflakeService) : base(snowflakeService) { }

    public override IStructure Generate() 
    {
        if (Teams.Count == 0)
            throw new InvalidOperationException("No teams are currently in the bracket builder");

        //// Get the number of teams in lower and upper rounds (probably not needed)
        //int maxDepth = (int)Math.Ceiling(Math.Log2(Teams.Count));
        //int lowerSlots = (int)Math.Pow(2, maxDepth);
        //int upperSlots = lowerSlots / 2;

        //int needsForLower = Teams.Count - upperSlots;
        //int teamsInLower = needsForLower * 2;
        //int teamsInUpper = Teams.Count - teamsInLower;

        //Console.WriteLine($"Of {Teams.Count} teams, {teamsInLower} are in lower and {teamsInUpper} are in upper.");

        Team[] teams = GetOrderedTeams();

        /*
         * TODO: Generate the bracket using the seeded teams
         * 
         * Assuming the teams are in order of best to worst, the new team
         * is put against the worst team at upper round of the bracket.
         */

        IStructure root = new Finale(new(SnowflakeService.Generate().ToString(), null, BestOf), new(1), new(2));

        BitArray higherRoundContainsIndex = new(2);
        
        for (int i = 0; i < teams.Length; i++)
        {
            // Reset round team contents when new depth is required
            if (Math.Ceiling(Math.Log2(i)) == Math.Floor(Math.Log2(i)))
                higherRoundContainsIndex = new((int)Math.Pow(2, Math.Ceiling(Math.Log2(i))), true);

            // Place the team in the bracket
            if (i < 2)
            {
                root.Series.AddTeam(teams[i]);
            }
            else
            {
                // Get index of team they need to compete against
                int j = higherRoundContainsIndex.Length;

                while (j > 0)
                    if (higherRoundContainsIndex[--j])
                        break;

                // Remove opponent from their current match
                IStructure head = root.FindStructureWithTeam(teams[j].Id)!;
                head.Series.RemoveTeam(teams[j].Id);

                // Create new series with the team and their opponent
                Dictionary<string, Team> seriesTeams = new();
                seriesTeams.Add(teams[i].Id, teams[i]);
                seriesTeams.Add(teams[j].Id, teams[j]);

                Series series = new(SnowflakeService.Generate().ToString(), seriesTeams, BestOf);
                IPlacement placement = new GroupPlacement((int)Math.Pow(2, Math.Ceiling(Math.Log2(i + 1))));

                // Build a structure for the series and join with current tree
                IStructure structure = new Structure(series);
                structure.SetWinnerProgression(head.Series);
                structure.SetLoserProgression(placement);
                head.AddChild(structure);

                // Move index of opponent from higher to lower round memory
                higherRoundContainsIndex[j] = false;
            }
        }

        // Return the root of the bracket tree
        return root;
    }
}
