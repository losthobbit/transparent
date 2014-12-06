using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    /// <summary>
    /// Inherit from this class in order to persist state in a string dictionary.
    /// </summary>
    public class Stateful
    {
        public string State { get; set; }

        public Stateful()
        {

        }

        public Stateful(string state)
        {
            State = state;
        }

        /// <summary>
        /// Creates a generic Stateful object with a clone of the state.
        /// </summary>
        /// <returns>A generic Stateful object with a clone of the state.</returns>
        public Stateful GetState()
        {
            return GetState<Stateful>();
        }

        /// <summary>
        /// Creates a generic Stateful object with a clone of the state.
        /// </summary>
        /// <returns>A generic Stateful object with a clone of the state.</returns>
        public T GetState<T>()
            where T: Stateful, new()
        {
            return new T
            {
                State = State
            };
        }

        protected List<StateKeyValuePair> StatePairs
        {
            get
            {
                if (string.IsNullOrEmpty(State))
                    State = "[]";
                return JsonConvert.DeserializeObject<List<StateKeyValuePair>>(State);
            }
            set
            {
                State = JsonConvert.SerializeObject(value);
            }
        }

        public StateKeyValuePair GetStateKeyValuePair(string key)
        {
            return StatePairs.FirstOrDefault(statePair => statePair.Key == key);
        }

        public string GetValue(string key)
        {
            var pair = GetStateKeyValuePair(key);
            return pair == null ? null : pair.Value;
        }

        public void SetValue(string key, string value)
        {
            var statePairs = StatePairs;
            var pair = statePairs.FirstOrDefault(statePair => statePair.Key == key);
            if (pair == null)
            {
                pair = new StateKeyValuePair { Key = key };
                statePairs.Add(pair);
            }
            pair.Value = value;
            StatePairs = statePairs;
        }

        public int GetInt(string key)
        {
            int value;
            return int.TryParse(GetValue(key), out value) ? value : 0;
        }

        public void SetInt(string key, int value)
        {
            SetValue(key, value.ToString());
        }

    }

    public class StateKeyValuePair
    {
        public string Key;
        public string Value;
    }

}
