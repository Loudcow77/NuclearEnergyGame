using System.Collections.Generic;
using UnityEngine;

namespace AwesomeCharts {
    [System.Serializable]
    public class BasicValueFormatterConfig {

        [SerializeField]
        private int valueDecimalPlaces = 1;
        [SerializeField]
        public List<string> customValues;

        public int ValueDecimalPlaces {
            get { return valueDecimalPlaces; }
            set { valueDecimalPlaces = value; }
        }

        public List<string> CustomValues {
            get { return customValues; }
            set { customValues = value; }
        }
    }
}