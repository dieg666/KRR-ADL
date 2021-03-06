using System;
using System.Collections.Generic;
using System.Linq;

namespace ADLModels
{
    public class Action : IEquatable<Action>
    {
        public string Name { get; }
        public int Duration { get; }

        public Action(string name)
        {
            Name = name;
        }
        public Action(string name, int duration)
        {
            Name = name;
            Duration = duration;
        }
        public new string ToString()
        {
            return Name;
        }

        public bool Equals(Action other)
        {
            return Name == other.Name;
        }

        public static implicit operator Action(string str)
        {
            return new Action(str);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public static class ActionsList
    {
        public static string ToProlog(this List<Action> list)
        {
            return $"[{list.OrderBy(x => x.Name).Select(x => x.Name).Aggregate((a, b) => a + ", " + b)}]";
        }
    }
}