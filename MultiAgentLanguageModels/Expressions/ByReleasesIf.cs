using System.Linq;

namespace ADLModels.Expressions
{
    public class ByReleasesIf : Expression
    {
        public Action Action { get; }
        public Fluent Fluent { get; }
        public LogicExpression Condition { get; }
        public ByReleasesIf(Action action, Fluent fluent, LogicExpression condition)
        {
            Action = action;
            Fluent = fluent;
            Condition = condition;
        }
    }

    public class ByReleases : ByReleasesIf
    {
        public ByReleases(Action action, Fluent fluent)
            : base(action, fluent, new True())
        {

        }
    }

    public class ReleasesIf : ByReleasesIf
    {
        public ReleasesIf(Action action, Fluent fluent, LogicExpression condition)
            : base(action, fluent, condition)
        {

        }
    }

    public class Releases : ByReleasesIf
    {
        public Releases(Action action, Fluent fluent)
            : base(action, fluent, new True())
        {

        }
    }
}