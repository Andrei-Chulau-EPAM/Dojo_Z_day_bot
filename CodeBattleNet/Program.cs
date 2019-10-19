using System;
using System.Diagnostics;
using CodeBattleNet.AI;
using CodeBattleNet.Analitics;
using CodeBattleNet.Core;
using CodeBattleNet.Utilites;
using CodeBattleNetLibrary;

namespace CodeBattleNet
{
    internal static class Program
    {
        private static void Main()
        {
            var client = new BattleNetClient(
                "http://dojorena.io/codenjoy-contest/board/player/ix61eg04ddy0dtfh3ckq?code=382142621680346506&gameName=battlecity");
            var analitics = new RegionAnalysis(client);
            var fightProgram = new FightProgram(client);
            var moveProgram = new MoveProgram(client);
            var wayOutProgram = new WayOutProgram(client);
            var defenseProgram = new DefenseProgram(client);
            var commandsUtility = new CommandsProcessor(client);
            var logger = new NLog.LogFactory().GetCurrentClassLogger();

            client.Run(() =>
            {
                try
                {
                    Stopwatch timer = Stopwatch.StartNew();

                    if (!client.IsPlayerAlive())
                    {
                        client.SendActions(client.Blank());
                        logger.Debug("Our tank had been destroyed");
                        return;
                    }
                    logger.Debug($"Current coordinates [{client.PlayerX}, {client.PlayerY}]");
                    //var freeRegion = analitics.GetFreeRegion();
                    var freeRegion = new Region(new Region(client.PlayerY, client.PlayerY, client.PlayerX, client.PlayerX), 6);
                    //logger.Debug($"Region analitics finished {freeRegion.ToString()}");
                    //var enemiesInArea = client.GetEnemiesPoints(freeRegion);
                    var enemiesInArea = client.GetEnemiesPoints(freeRegion);
                    logger.Debug($"Enemy search region {freeRegion.ToString()}");
                    var enemiesCount = enemiesInArea.Count;
                    logger.Debug($"Enemies {enemiesCount} found at region");

                    if (enemiesCount > 0)
                    {
                        logger.Debug($"Closest enemy at {analitics.GetClosestEnemy(freeRegion)} found at region");
                    }

                    var preAct = false;
                    var movement = Movement.Stop;
                    var postAct = false;
                    if (defenseProgram.CanBeUsed())
                    {
                        logger.Debug("Defense mode used");
                        movement = moveProgram.GetSafeMovementDirection(defenseProgram.DangerDirections);

                    }
                    else if (fightProgram.CanBeUsed(6))
                    {
                        logger.Debug("Fight mode used");
                        //preAct = fightProgram.TryPreAct(enemiesInArea);
                        movement = moveProgram.GetMovementToClosestEnemy();
                        postAct = fightProgram.TryPostAct(movement.ToDirection(client), enemiesInArea);
                    }
                    else if (wayOutProgram.CanBeUsed(6))
                    {
                        logger.Debug("WayOut mode used");
                        //preAct = wayOutProgram.TryPreAct(freeRegion);
                        movement = moveProgram.GetMovementToClosestConstruction();
                        postAct = true; //wayOutProgram.TryPostAct(movement.ToDirection(client), freeRegion);
                    }
                    else
                    {
                        logger.Debug("default mode used");
                        //Move(client);
                        movement = moveProgram.GetLongTermRandomDirection();
                    }

                    var command = commandsUtility.CreateCommand(preAct, movement, postAct);
                    logger.Debug($"Command sending [{command}]");
                    client.SendActions(command);
                    timer.Stop();
                    logger.Debug($"Turn finished in {timer.ElapsedMilliseconds} ms");
                }
                catch (Exception e)
                {
                    logger.Fatal(e, "Programm ended\n");
                    throw;
                }
            });
            Console.Read();
        }

        private static void Move(BattleNetClient gcb)
        {
            var r = new Random();
            var done = false;

            switch (r.Next(5))
            {
                case 0:
                    if (!gcb.IsBarrierAt(gcb.PlayerX, gcb.PlayerY - 1))
                    {
                        gcb.SendActions(gcb.Up());
                        Console.WriteLine($"Move up");
                        done = true;
                    }
                    break;
                case 1:
                    if (!gcb.IsBarrierAt(gcb.PlayerX + 1, gcb.PlayerY))
                    {
                        gcb.SendActions(gcb.Right());
                        Console.WriteLine($"Move right");
                        done = true;
                    }
                    break;
                case 2:
                    if (!gcb.IsBarrierAt(gcb.PlayerX, gcb.PlayerY + 1))
                    {
                        gcb.SendActions(gcb.Down());
                        Console.WriteLine($"Move down");
                        done = true;
                    }
                    break;
                case 3:
                    if (!gcb.IsBarrierAt(gcb.PlayerX - 1, gcb.PlayerY))
                    {
                        gcb.SendActions(gcb.Left());
                        Console.WriteLine($"Move left");
                        done = true;
                    }
                    break;
                case 4:
                    gcb.SendActions(gcb.Act());
                    Console.WriteLine($"act");
                    done = true;
                    break;
            }
            if (done == false)
                gcb.SendActions(gcb.Blank());
        }
    }
}
