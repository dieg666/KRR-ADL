using System;
using System.Collections.Generic;
using System.Linq;

namespace ADLModels.Expressions
{
    public class ByCausesIf : Expression
    {
        public Action A { get; }


        public LogicExpression Pi { get; }

        public LogicExpression Alpha { get; }

        public ByCausesIf(Action action, LogicExpression result, LogicExpression condition)
        {
            A = action;
            Pi = condition;
            Alpha = result;
        }
    }

    public class ByCauses : ByCausesIf
    {
        public ByCauses(Action action, LogicExpression result)
            :base(action,  result, new True())
        {
        }
    }

    public class CausesIf : ByCausesIf
    {
        public CausesIf(Action action, LogicExpression result, LogicExpression condition)
            : base(action, result, condition)
        {
        }
    }

    public class Causes : ByCausesIf
    {
        public Causes(Action action, LogicExpression result)
            : base(action, result, new True())
        {
        }
    }

    public class ImpossibleByIf : ByCausesIf
    {
        public ImpossibleByIf(Action action, LogicExpression condition) : base(action, new False(), condition)
        {
        }
    }

    public class ImpossibleBy : ByCausesIf
    {
        public ImpossibleBy(Action action) : base(action, new False(), new True())
        {
        }
    }

    public class ImpossibleIf : ByCausesIf
    {
        public ImpossibleIf(Action action, LogicExpression condition) : base(action, new False(), condition)
        {
        }
    }
}