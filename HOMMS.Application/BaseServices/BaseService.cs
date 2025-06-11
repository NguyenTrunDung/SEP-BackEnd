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
        protected int EnsureBranchId(int branchId)
        {
            return branchId == 0 ? BranchContext.GetCurrentBranchId() : branchId;
        }
    }
}
