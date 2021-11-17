using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testlet.Tests
{
    [TestClass]
    public class TestletTests
    {
        private List<Item> ItemsSet { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            ItemsSet = GetItemsSet();
        }

        [TestMethod]
        public void Expect_Same_Elements_Count_As_Items_Set()
        {
            // Arrange
            var testlet = new Testlet("test", ItemsSet);

            // Act
            var actual = testlet.Randomize();

            // Assert
            Assert.AreEqual(ItemsSet.Count, actual.Count);
        }

        [TestMethod]
        public void Expect_Elements_From_The_Items_Set()
        {
            // Arrange
            var testlet = new Testlet("test", ItemsSet);

            // Act
            var actual = testlet.Randomize();

            // Assert
            actual.ForEach(item => Assert.IsTrue(ItemsSet.Contains(item)));
        }
        
        [TestMethod]
        public void Expect_Pretest_Items_As_The_First_Elements()
        {
            // Arrange
            var testlet = new Testlet("test", ItemsSet);

            // Act
            var actual = testlet.Randomize();

            // Assert
            for (int i = 0; i < TestletItemsCount.FirstPretestItems; i++)
            {
                Assert.AreEqual(ItemTypeEnum.Pretest, actual[i].ItemType);
            }
        }

        [TestMethod]
        public void Expect_Correct_Number_Of_Pretest_And_Operational_Items_As_The_Remaining_Elements()
        {
            // Arrange
            var testlet = new Testlet("test", ItemsSet);

            // Act
            var randomizedItems = testlet.Randomize();
            var remainingRandomizedItemsByType = randomizedItems.Skip(TestletItemsCount.FirstPretestItems).ToLookup(i => i.ItemType);
            var expectedRemainingPretestElementsCount = TestletItemsCount.Pretest - TestletItemsCount.FirstPretestItems;

            // Assert
            Assert.AreEqual(expectedRemainingPretestElementsCount, remainingRandomizedItemsByType[ItemTypeEnum.Pretest].Count());
            Assert.AreEqual(TestletItemsCount.Operational, remainingRandomizedItemsByType[ItemTypeEnum.Operational].Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), nameof(Testlet.TestletId))]
        public void Expect_Argument_Null_Exception_Thrown_For_Null_TestletId()
        {
            var testlet = new Testlet(null, ItemsSet);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), nameof(Testlet.Items))]
        public void Expect_Argument_Null_Exception_Thrown_For_Null_TestItems()
        {
            var testlet = new Testlet("test", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), nameof(Testlet.Items))]
        public void Expect_Argument_Exception_Thrown_For_Surplus_TestItems_Count()
        {
            ItemsSet.Add(new Item
            {
                ItemId = "ExtraItemId",
                ItemType = ItemTypeEnum.Pretest
            });
            var testlet = new Testlet("test", ItemsSet);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), nameof(Testlet.Items))]
        public void Expect_Argument_Exception_Thrown_For_Insufficient_TestItems_Count()
        {
            ItemsSet.RemoveAt(ItemsSet.Count - 1);
            var testlet = new Testlet("test", ItemsSet);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), nameof(Testlet.Items))]
        public void Expect_Argument_Exception_Thrown_For_Wrong_TestItems_Type_Count()
        {
            var pretestItem = ItemsSet.First(item => item.ItemType == ItemTypeEnum.Pretest);
            pretestItem.ItemType = ItemTypeEnum.Operational;
            var testlet = new Testlet("test", ItemsSet);
        }

        private List<Item> GetItemsSet()
        {
            var resultSet = new List<Item>(TestletItemsCount.Operational + TestletItemsCount.Pretest);
            resultSet
                .AddRange(Enumerable.Range(1, TestletItemsCount.Pretest)
                    .Select(x => new Item
                    {
                        ItemId = $"Pretest{x}",
                        ItemType = ItemTypeEnum.Pretest
                    }));

            resultSet
                .AddRange(Enumerable.Range(TestletItemsCount.Pretest + 1, TestletItemsCount.Operational)
                    .Select(x => new Item
                    {
                        ItemId = $"Operational{x}",
                        ItemType = ItemTypeEnum.Operational
                    }));

            return resultSet;
        }
    }
}