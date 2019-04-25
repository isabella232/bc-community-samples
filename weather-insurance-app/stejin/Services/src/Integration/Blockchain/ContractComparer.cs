using System;
using System.Linq;
using Nethereum.ABI.Model;
using Nethereum.Contracts;

namespace WeatherInsurance.Integration.Blockchain
{
public class ContractComparer
    {
        private readonly Contract _reference;
        private readonly Contract _candidate;

        public ContractComparer(string referenceAbi, string candidateAbi)
        {
            _reference = new Contract(null, referenceAbi, string.Empty);
            _candidate = new Contract(null, candidateAbi, string.Empty);
        }


        public bool IsInterfaceImplemented()
        {
            if (_reference == null)
                return _candidate == null;

            if (_candidate == null)
                return false;

            var parameterComparer = new MultiSetComparer<Parameter>(new ParameterComparer());

            var referenceFunctions = _reference.ContractBuilder.ContractABI.Functions;
            foreach (var referenceFunction in referenceFunctions)
            {
                var candidateFunction = _candidate.ContractBuilder.ContractABI.Functions.FirstOrDefault(f => f.Name == referenceFunction.Name);
                if (candidateFunction == null)
                    return false;

                if (!parameterComparer.Equals(referenceFunction.InputParameters, candidateFunction.InputParameters))
                    return false;

                if (!parameterComparer.Equals(referenceFunction.OutputParameters, candidateFunction.OutputParameters))
                    return false;
            }

            var referenceEvents = _reference.ContractBuilder.ContractABI.Events;
            foreach (var referenceEvent in referenceEvents)
            {
                var candidateEvent = _candidate.ContractBuilder.ContractABI.Events.FirstOrDefault(e => e.Name == referenceEvent.Name);
                if (candidateEvent == null)
                    return false;

                if (!parameterComparer.Equals(referenceEvent.InputParameters, candidateEvent.InputParameters))
                    return false;
            }

            return true;
        }


        public int GetHashCode(Contract obj)
        {
            return obj.GetHashCode();
        }

        
    }
}
