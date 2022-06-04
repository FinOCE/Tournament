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

        //Series series = new(SnowflakeService.Generate().ToString(), null, BestOf);
        //IStructure root = new Finale(series, new(1), new(2));

        //IStructure head = root;
        //foreach (Team team in teams)
        //{
        //    if (head.Series.Teams.Count < 2)
        //        head.Series.AddTeam(team);
        //    else
        //}

        // Return the root of the bracket tree
        return new Structure(new Series(SnowflakeService.Generate().ToString(), null, 1)); // TEMPORARY TO FIT ABSTRACT
    }
}
