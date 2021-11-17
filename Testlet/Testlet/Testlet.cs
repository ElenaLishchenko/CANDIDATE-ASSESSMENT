using System;
using System.Collections.Generic;
using System.Linq;

namespace Testlet
{
    public class Testlet
    {
        public Testlet(string testletId, List<Item> items)
        {
            TestletId = testletId ?? throw new ArgumentNullException(nameof(TestletId));
            Items = items ?? throw new ArgumentNullException(nameof(Items));

            if (!IsItemsCountValid())
            {
                throw new ArgumentException(nameof(Items));
            }
        }

        public string TestletId { get; set; }
        public List<Item> Items { get; set; }

        public List<Item> Randomize()
        {
            var result = CreateShuffledItemsListCopy(Items);
            MovePretestItemsToBeginning(result);
            return result;
        }

        private bool IsItemsCountValid()
        {
            var pretestItemsCount = Items.Count(item => item.ItemType == ItemTypeEnum.Pretest);
            var operationalItemsCount = Items.Count(item => item.ItemType == ItemTypeEnum.Operational);
            if (operationalItemsCount != TestletItemsCount.Operational
                || pretestItemsCount != TestletItemsCount.Pretest)
            {
                return false;
            }

            return true;
        }

        private List<Item> CreateShuffledItemsListCopy(List<Item> items)
        {
            var itemsListCopy = new List<Item>(items);
            Shuffle(itemsListCopy);
            return itemsListCopy;
        }

        private void MovePretestItemsToBeginning(List<Item> items)
        {
            var pretestItemsAtTheBeginningCount = 0;

            for (var index = 0; index < items.Count; index++)
            {
                var item = items[index];
                if (item.ItemType != ItemTypeEnum.Pretest)
                {
                    continue;
                }
                Swap(items, pretestItemsAtTheBeginningCount, index);
                pretestItemsAtTheBeginningCount++;
                if (pretestItemsAtTheBeginningCount == TestletItemsCount.FirstPretestItems)
                {
                    break;
                }
            }
        }

        private void Shuffle(List<Item> items)
        {
            Random random = new Random();
            for (var i = items.Count - 1; i >= 0; i--)
            {
                var randomIndex = random.Next(i + 1);
                Swap(items, randomIndex, i);
            }
        }

        private void Swap(List<Item> items, int newIndex, int index)
        {
            var value = items[newIndex];
            items[newIndex] = items[index];
            items[index] = value;
        }
    }
}
