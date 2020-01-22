using System;
using Dapper;

namespace Dwapi.SharedKernel.Infrastructure.Tests.TestHelpers
{
    public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override Guid Parse(object value)
        {
            var inVal = (byte[])value;
            return new Guid(inVal);
        }

        public override void SetValue(System.Data.IDbDataParameter parameter, Guid value)
        {
            var inVal = value.ToByteArray();
            parameter.Value = inVal;
        }
    }
}