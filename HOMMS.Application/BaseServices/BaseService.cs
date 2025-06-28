using HOMMS.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.BaseServices
{
    public abstract class BaseService
    {
        protected readonly IBranchContext BranchContext;
        protected BaseService(IBranchContext branchContext)
        {
            BranchContext = branchContext;
        }
        protected int EnsureBranchId(int? branchId)
        {
            return branchId.HasValue && branchId.Value != 0
                ? branchId.Value
                : BranchContext.GetCurrentBranchId();
        }
        protected int EnsureBranchId2()
        {
            return  BranchContext.GetCurrentBranchId();
        }
    }
}
