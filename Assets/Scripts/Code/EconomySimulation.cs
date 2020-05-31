using System;
using System.Collections.Generic;

// Simulates the economy of the game.
// It holds the current amount of available money.
// The econonomy is handled in cycles:
// During each cycle, income is added and upkeep costs are subtracted.
public class EconomySimulation
{
    // The current amount of available money
    public int AvailableMoney { get; private set; }

    private Func<int> getIncome;
    private Func<List<int>> getUpkeepCosts;

    // initialize simulation
    // initialMoney:
    //  The initial amount of available money at the start of the simulation.
    // getIncome: Called once per cycle to get the income for the cycle.
    // getUpkeepCosts:
    //  Called once per cycle to get all upkeep costs for the cycle.
    public EconomySimulation(
        int initialMoney,
        Func<int> getIncome,
        Func<List<int>> getUpkeepCosts
    )
    {
        AvailableMoney = initialMoney;
        this.getIncome = getIncome;
        this.getUpkeepCosts = getUpkeepCosts;
    }

    // Spend some amount of money.
    public void SpendMoney(int amount)
    {
        // is enough money available?
        if (!CanAfford(amount))
            throw new ArgumentOutOfRangeException(
                paramName: "amount",
                message: "Cannot spend more money than available."
            );

        AvailableMoney -= amount;
    }

    // Return whether you can afford to spend a certain amount of money.
    public bool CanAfford(int amount)
    {
        // catch negative amount
        if (amount < 0)
            throw new ArgumentOutOfRangeException(
                paramName: "amount",
                message: "Cannot spend negative amount of money."
            );

        bool canAfford = (amount <= AvailableMoney);
        return canAfford;
    }

    // Run one simulation cycle
    public void Tick()
    {
        handleIncome();
        handleUpkeepCosts();
    }

    // Handle income in simulation cycle.
    private void handleIncome()
    {
        if (getIncome != null)
        {
            int income = getIncome();
            if (income < 0)
                throw new ArgumentOutOfRangeException(
                    paramName: "amount",
                    message: "Cannot have negative income."
                );
            AvailableMoney += income;
        }
    }

    // Handle upkeep costs in simulation cycle.
    private void handleUpkeepCosts()
    {
        if (getUpkeepCosts == null)
            return;

        var costs = getUpkeepCosts();
        foreach (var cost in costs)
        {
            AvailableMoney = Math.Max(AvailableMoney - cost, 0);
        }
    }
}

