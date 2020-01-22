using System;

namespace Dwapi.UploadManagement.Core.Model
{
    public class PackagePager
    {
        public int PageCount(int batchSize, int totalRecords)
        {
            if (totalRecords > 0) {
                if (totalRecords < batchSize) {
                    return 1;
                }
                var pgs=(totalRecords % batchSize);
                double wh = (double)totalRecords /(double)batchSize;
                return (int) (Math.Floor(wh) + pgs);

            }
            return 0;
        }
    }
}
