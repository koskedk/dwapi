using Dapper;

namespace Dwapi.SharedKernel.Infrastructure.Tests.TestHelpers
{
    public class NumericTypeHandler : SqlMapper.TypeHandler<decimal?>
    {
        public override decimal? Parse(object value)
        {
            if (decimal.TryParse(value.ToString(), out var d))
                return d;
            return 0;
        }

        public override void SetValue(System.Data.IDbDataParameter parameter, decimal? value)
        {
            parameter.Value = value;
        }
    }
}
