using System;

namespace PaymentsSystemExample.Api.Extensions
{
    public static class StringExtensions
    {
        public static Guid TryConvertIdToGuid(this string id) 
        {
            Guid outGuid;

            if(Guid.TryParse(id, out outGuid))
            {
                return outGuid;
            }

            return default(Guid);
        }
    }
}