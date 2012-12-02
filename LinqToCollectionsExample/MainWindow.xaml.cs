using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Strilanc.LinqToCollections;

namespace LinqToCollectionsExample {
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();

            // all of these examples create lists with constant-time operations that don't get more expensive as the list grows.
            // these operations include:
            // - creating the list
            // - determining the number of items in the list
            // - accessing items by index in the list
            // - advancing an enumeration of the list
            // also, only a constant amount of memory is used per list (except for what's used to show their items as text)

            Show("Naturals less than A", (a, b, c) =>
                a.Range());
            
            Show("B", (a, b, c) => 
                b);
            
            Show("C", (a, b, c) => 
                c);

            Show("Last A bytes", (a, b, c) =>
                ReadOnlyList.AllBytes().TakeLast(a));

            Show("B reversed", (a, b, c) =>
                b.Reverse());

            Show("Take up to A of C", (a, b, c) =>
                c.Take(a));

            Show("Stride to a million by A", (a, b, c) =>
                1000000.Range().Stride(a));

            Show("C with item letters sorted", (a, b, c) =>
                c.Select(e => new String(e.OrderBy(character => character).ToArray())));

            Show("Pairwise equals of B,C", (a, b, c) =>
                b.Zip(c, (itemB, itemC) => itemB == itemC));

            Show("Skip first and last A of B", (a, b, c) =>
                b.Skip(a).SkipLast(a));

            Show("Skip EXACTLY last A of B", (a, b, c) =>
                b.SkipLastRequire(a));

            Show("Partition C by A, concatenating", (a, b, c) => 
                from g in c.Partition(a) 
                select String.Join("", g));

            Show("Cube map", (a, b, c) => {
                var count = 100;
                var squareMap = new AnonymousReadOnlyDictionary<int, int>(
                    count.Range(),
                    (int key, out int value) => {
                        value = key*key;
                        return key >= 0 && key < count;
                    });
                var cubeMap = squareMap.Select(e => e.Key*e.Value);
                return cubeMap; // treated as an IReadOnlyCollection<KeyValuePair<int, int>>
            });
        }

        private void Show<T>(string title, Func<int, IReadOnlyList<string>, IReadOnlyList<string>, IReadOnlyCollection<T>> computation) {
            // create controls to show result
            var rowColor = Rows.Children.Count % 2 == 0 ? Color.FromArgb(0, 0, 0, 0) : Color.FromArgb(25, 0, 0, 0);
            var rowStack = new StackPanel { Orientation = Orientation.Horizontal, Background = new SolidColorBrush(rowColor) };
            var titleBlock = new TextBlock { Width = 200, Text = title };
            var countBlock = new TextBlock { Width = 100 };
            var itemsBlock = new TextBlock();
            rowStack.Children.Add(titleBlock);
            rowStack.Children.Add(countBlock);
            rowStack.Children.Add(itemsBlock);
            Rows.Children.Add(rowStack);

            Action update = () => {
                try {
                    var ci = computation((int)SliderA.Value, TextB.Text.Split(','), TextC.Text.Split(','));
                    var cutOffLength = 200;
                    countBlock.Text = "Count: " + ci.Count;
                    itemsBlock.Text = String.Format(
                        "Items: [{0}{1}",
                        String.Join(", ", ci.Take(cutOffLength)),
                        ci.Count <= cutOffLength ? "]" : ", ...");
                } catch (Exception ex) {
                    countBlock.Text = "error";
                    itemsBlock.Text = ex.Message.Split(new[] {Environment.NewLine}, StringSplitOptions.None).First();
                }
            };

            // show initial value
            update();
            // recompute when input changes
            SliderA.ValueChanged += (sender, arg) => update();
            TextB.TextChanged += (sender, arg) => update();
            TextC.TextChanged += (sender, arg) => update();
        }
    }
}
