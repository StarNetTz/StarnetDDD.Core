using System;
using System.Collections.Generic;
using System.Reflection;

namespace Starnet.Aggregates
{
    public class AggregateStateFactory
    {
        static Dictionary<Type, Type> LookupTable;

        static AggregateStateFactory()
        {
            LookupTable = new Dictionary<Type, Type>();
        }

        public static IAggregateState CreateStateFor(Type aggregateType)
        {
            Type aggStateType = null;
            if (!LookupTable.TryGetValue(aggregateType, out aggStateType))
            {
                Assembly assemblyThatContainsAggregate = aggregateType.Assembly;
                string aggStateTypeName = string.Format("{0}State", aggregateType.FullName);
                aggStateType = assemblyThatContainsAggregate.GetType(aggStateTypeName);
                LookupTable[aggregateType] = aggStateType;
            }
            var obj = Activator.CreateInstance(aggStateType);
            return obj as IAggregateState;
        }
    }
}