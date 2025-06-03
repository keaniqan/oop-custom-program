
using NUnit.Framework;
#nullable disable

namespace MyApp.Tests
{
    [TestFixture]
    public class AttackActionTest
    {
        [Test]
        public void TestAttackCommand()
        {
            // setup
            var player = new Player("TestPlayer", 50, 50, 0, 3, new List<Effect>(), 0, new List<Charm>(), new List<Card>(), 0.0);
            var enemy = new Enemy("TestEnemy", 30, 30, 0, new List<Effect>(), EnemyType.Basic, 1);
            var game = new Game(player);

            //execute
            var command = new AttackCommand(10, EffectTarget.Enemy);
            command.Execute(player, enemy, game);

            //assert
            Assert.AreEqual(20, enemy.Health, "Enemy should take 10 damage from AttackCommand");
        }
    }

    public class BlockActionTest
    {
        [Test]
        public void TestBlockCommand()
        {
            // setup
            var player = new Player("TestPlayer", 50, 50, 0, 3, new List<Effect>(), 0, new List<Charm>(), new List<Card>(), 0.0);
            var enemy = new Enemy("TestEnemy", 30, 30, 0, new List<Effect>(), EnemyType.Basic, 1);
            var game = new Game(player);

            //execute
            var command = new BlockCommand(5);
            command.Execute(player, enemy, game);

            //assert
            Assert.AreEqual(5, player.Block, "Player should gain 5 block from BlockCommand");
        }
    }

    public class ApplyEffectActionTest
    {
        [Test]
        public void TestApplyEffectCommand()
        {
            // setup
            var player = new Player("TestPlayer", 50, 50, 0, 3, new List<Effect>(), 0, new List<Charm>(), new List<Card>(), 0.0);
            var enemy = new Enemy("TestEnemy", 30, 30, 0, new List<Effect>(), EnemyType.Basic, 1);
            var game = new Game(player);

            //execute
            var command = new ApplyEffectCommand(EffectType.StrengthUp, 1, EffectTarget.Player);
            command.Execute(player, enemy, game);

            //assert
            Assert.AreEqual(1, player.Effects.First(e => e.EffectType == EffectType.StrengthUp).Stacks, "Player should have 1 strength up effect");
        }
    }
    public class DrawActionTest
    {
        [Test]
        public void TestDrawCommand()
        {
            // setup
            var player = new Player("TestPlayer", 50, 50, 0, 3, new List<Effect>(), 0, new List<Charm>(), new List<Card>(), 0.0);
            var testcard = new Card("TestCard", "TestDescription", 0, new List<ActionCommand>(), CardLocation.DrawPile, 0);
            player.AddCard(testcard);
            var enemy = new Enemy("TestEnemy", 30, 30, 0, new List<Effect>(), EnemyType.Basic, 1);
            var game = new Game(player);

            //execute
            var command = new DrawCommand(1);
            command.Execute(player, enemy, game);

            //assert
            //loop through player.Cards and check how many cards position is Hand
            int handCount = 0;
            foreach (var card in player.Cards)
            {
                if (card.CardLocation == CardLocation.Hand)
                {
                    handCount++;
                }
            }
            Assert.AreEqual(1, handCount, "Player should draw 1 card from DrawCommand");
        }
    }
    public class GainEnergyActionTest
    {
        [Test]
        public void TestGainEnergyCommand()
        {
            // setup
            var player = new Player("TestPlayer", 50, 50, 0, 3, new List<Effect>(), 0, new List<Charm>(), new List<Card>(), 0.0);
            var enemy = new Enemy("TestEnemy", 30, 30, 0, new List<Effect>(), EnemyType.Basic, 1);
            var game = new Game(player);

            //execute
            var command = new GainEnergyCommand(1);
            command.Execute(player, enemy, game);

            //assert
            if (GameRenderer.game?.CurrentRoom is Combat combatRoom)
            {
                Assert.AreEqual(1, combatRoom.CurrentEnergy, "Player should gain 1 energy from GainEnergyCommand");
            }
        }
    }
    public class HealActionTest
    {
        [Test]
        public void TestHealCommand()
        {
            // setup
            var player = new Player("TestPlayer", 49, 50, 0, 3, new List<Effect>(), 0, new List<Charm>(), new List<Card>(), 0.0);
            var enemy = new Enemy("TestEnemy", 30, 30, 0, new List<Effect>(), EnemyType.Basic, 1);
            var game = new Game(player);

            //execute
            var command = new HealCommand(1);
            command.Execute(player, enemy, game);

            //assert
            Assert.AreEqual(50, player.Health, "Player should heal 1 health from HealCommand");
        }
    }
    public class SelfDamageActionTest
    {
        [Test]
        public void TestSelfDamageCommand()
        {
            // setup
            var player = new Player("TestPlayer", 50, 50, 0, 3, new List<Effect>(), 0, new List<Charm>(), new List<Card>(), 0.0);
            var enemy = new Enemy("TestEnemy", 30, 30, 0, new List<Effect>(), EnemyType.Basic, 1);
            var game = new Game(player);

            //execute
            var command = new SelfDamageCommand(1);
            command.Execute(player, enemy, game);

            //assert
            Assert.AreEqual(49, player.Health, "Player should take 1 damage from SelfDamageCommand");
        }
    }
}
