using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using Microsoft.UI.Xaml;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TreeViewSelectionRepro;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
    }

    public ObservableCollection<Person> People { get; set; } =
        [
            new Person("ParentA", new("ChildA"), new("ChildB")),
            new Person("ParentB", new("ChildC"), new("ChildD", new Person("GrandChildA"))),
            new Person("ParentC", new("ChildE"), new("ChildF") { IsSelected = true }),
        ];
}

public class Person : INotifyPropertyChanged
{
    public Person(string name, params Person[]? children)
    {
        Name = name;

        if (children != null)
            foreach (var child in children!)
                Children.Add(child);
    }

    public ObservableCollection<Person> Children { get; } = new();

    public string Name { get; }

    bool _IsSelected;
    public bool IsSelected
    {
        get => _IsSelected;
        set
        {
            if (_IsSelected == value)
                return;

            _IsSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    public override string ToString() => Name;

    public override bool Equals(object? obj)
    {
        if (obj is Person person)
            return ReferenceEquals(this, person) || (Name == person.Name && Children.SequenceEqual(person.Children));

        return false;
    }

    public override int GetHashCode() => HashCode.Combine(Name, Children.Select(c => c.GetHashCode()).Aggregate(HashCode.Combine));
}
