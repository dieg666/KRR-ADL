using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiAgentLanguageModels.Expressions
{
    public class ExpressionsList : List<Expression>
    {
        public ExpressionsList()
        {
        }
        public ExpressionsList(IEnumerable<Fluent> fluents)
        {
            Fluent = fluents;
        }
        private IEnumerable<Fluent> Fluent { get; set; }
        public List<Action> Actions
        {
            get
            {
                List<Action> actions = new List<Action>();
                foreach (Expression ex in this)
                {
                    if (ex as ByCausesIf != null)
                    {
                        var temp = ex as ByCausesIf;
                        actions.Add(temp.A);
                    }
                    else if (ex as ByReleasesIf != null)
                    {
                        var temp = ex as ByReleasesIf;
                        actions.Add(temp.Action);
                    }

                }
                return actions.Distinct().ToList();
            }
        }
        public List<string> Fluents
        {
            get
            {
                List<string> fluents = new List<string>();
                if(!(Fluent is null))
                {
                    fluents.AddRange(Fluent.Select(x => x.Name));
                    return fluents.Distinct().ToList();
                }
                foreach (Expression ex in this)
                {
                    if (ex as ByCausesIf != null)
                    {
                        var temp = ex as ByCausesIf;
                        fluents.AddRange(temp.Pi.Fluents.Select(x => x.Key));
                        fluents.AddRange(temp.Alpha.Fluents.Select(x => x.Key));
                    }
                    else if (ex as ByReleasesIf != null)
                    {
                        var temp = ex as ByReleasesIf;
                        fluents.AddRange(temp.Condition.Fluents.Select(x => x.Key));
                        fluents.Add(temp.Fluent.Name);
                    }
                }
                return fluents.Distinct().ToList();
            }
        }
        public List<Initially> Initially
        {
            get
            {
                return this.Where(x => x as Initially != null).Select(x => x as Initially).ToList();
            }
        }
        public List<ByCausesIf> Causes
        {
            get
            {
                return this.Where(x => x as ByCausesIf != null).Select(x => x as ByCausesIf).ToList();
            }
        }
        public List<ByReleasesIf> Releases
        {
            get
            {
                return this.Where(x => x as ByReleasesIf != null).Select(x => x as ByReleasesIf).ToList();
            }
        }




    }
}
