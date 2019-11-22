using System;
using System.Collections.Generic;
using Nethereum.ABI.Model;

namespace WeatherInsurance.Integration.Blockchain
{
    public class ParameterComparer : IEqualityComparer<Parameter>
    {

        public bool Equals(Parameter x, Parameter y)
        {
            return (x.Name == y.Name && x.Type == y.Type && x.Order == y.Order);
        }

        public int GetHashCode(Parameter obj)
        {
            var hashCodes = new List<int> { obj.Name.GetHashCode(), obj.Type.GetHashCode(), obj.Order.GetHashCode() };

            int hash = 17;

            foreach (int val in hashCodes)
                hash = hash * 23 + val;

            return hash;
        }
    }
}
