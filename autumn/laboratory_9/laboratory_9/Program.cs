using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

public class Game
{
    public string Name { get; set; }
    public string Genre { get; set; }
    public int ReleaseYear { get; set; }

    public override string ToString()
    {
        return $"{Name} ({Genre}, {ReleaseYear})";
    }
}

public class GameCollection : IEnumerable<Game>
{
    private BlockingCollection<Game> _games = new BlockingCollection<Game>();

    public void AddGame(Game game) => _games.Add(game);

    public bool RemoveGame(Game game)
    {
        var tempList = _games.ToList();
        bool removed = tempList.Remove(game);
        if (removed)
        {
            _games = new BlockingCollection<Game>();
            foreach (var g in tempList)
                _games.Add(g);
        }
        return removed;
    }

    public Game FindGame(string name) => _games.FirstOrDefault(g => g.Name == name);

    public void PrintAll()
    {
        Console.WriteLine("Игры в коллекции:");
        foreach (var game in _games)
            Console.WriteLine($"  {game}");
    }

    public IEnumerator<Game> GetEnumerator() => _games.GetConsumingEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

class Program
{
    private static void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                Console.WriteLine($"Добавлен элемент: {(e.NewItems[0] as Game)?.Name}");
                break;
            case NotifyCollectionChangedAction.Remove:
                Console.WriteLine($"Удален элемент: {(e.OldItems[0] as Game)?.Name}");
                break;
            case NotifyCollectionChangedAction.Replace:
                Console.WriteLine($"Элемент заменен");
                break;
            case NotifyCollectionChangedAction.Reset:
                Console.WriteLine($"Коллекция очищена");
                break;
        }
    }

    static void Main()
    {
        Console.WriteLine("=== ЗАДАНИЕ 1: GameCollection с BlockingCollection ===");

        var gameCollection = new GameCollection();

        gameCollection.AddGame(new Game { Name = "The Witcher 3", Genre = "RPG", ReleaseYear = 2015 });
        gameCollection.AddGame(new Game { Name = "DOOM", Genre = "FPS", ReleaseYear = 2016 });
        gameCollection.AddGame(new Game { Name = "Civilization VI", Genre = "Strategy", ReleaseYear = 2016 });

        gameCollection.PrintAll();

        var foundGame = gameCollection.FindGame("DOOM");
        Console.WriteLine($"\nНайдена игра: {foundGame}");

        if (foundGame != null)
        {
            gameCollection.RemoveGame(foundGame);
            Console.WriteLine("После удаления DOOM:");
            gameCollection.PrintAll();
        }

        Console.WriteLine("\n=== ЗАДАНИЕ 2: Работа с универсальными коллекциями ===");

        Queue<int> queue = new Queue<int>();
        for (int i = 1; i <= 10; i++)
            queue.Enqueue(i * 10);

        Console.WriteLine("Исходная очередь: " + string.Join(", ", queue));

        int n = 3;
        Console.WriteLine($"\nУдаляем {n} элементов:");
        for (int i = 0; i < n && queue.Count > 0; i++)
        {
            int removed = queue.Dequeue();
            Console.WriteLine($"Удален: {removed}");
        }
        Console.WriteLine("Очередь после удаления: " + string.Join(", ", queue));

        queue.Enqueue(110); 
        Console.WriteLine($"После Enqueue(110): {string.Join(", ", queue)}");

        int[] newElements = { 120, 130, 140 };
        foreach (var element in newElements)
            queue.Enqueue(element);
        Console.WriteLine($"После добавления нескольких элементов: {string.Join(", ", queue)}");

        Dictionary<int, int> dictionary = new Dictionary<int, int>();
        int key = 1;
        foreach (var value in queue)
        {
            dictionary.Add(key++, value);
        }

        Console.WriteLine("\nСловарь (Dictionary):");
        foreach (var kvp in dictionary)
            Console.WriteLine($"  Ключ: {kvp.Key}, Значение: {kvp.Value}");

        int searchValue = 70;
        var foundPair = dictionary.FirstOrDefault(x => x.Value == searchValue);
        if (foundPair.Value == searchValue)
            Console.WriteLine($"\nЗначение {searchValue} найдено с ключом: {foundPair.Key}");
        else
            Console.WriteLine($"\nЗначение {searchValue} не найдено");

        Console.WriteLine("\n=== ЗАДАНИЕ 3: ObservableCollection ===");

        ObservableCollection<Game> observableGames = new ObservableCollection<Game>();
        observableGames.CollectionChanged += OnCollectionChanged;

        Console.WriteLine("\nДобавляем элементы в ObservableCollection:");
        observableGames.Add(new Game { Name = "Cyberpunk 2077", Genre = "RPG", ReleaseYear = 2020 });
        observableGames.Add(new Game { Name = "Halo Infinite", Genre = "FPS", ReleaseYear = 2021 });
        observableGames.Add(new Game { Name = "Starcraft 2", Genre = "Strategy", ReleaseYear = 2010 });

        Console.WriteLine("\nУдаляем элементы из ObservableCollection:");
        observableGames.RemoveAt(1); 

        Console.WriteLine("\nОчищаем коллекцию:");
        observableGames.Clear();
    }
}