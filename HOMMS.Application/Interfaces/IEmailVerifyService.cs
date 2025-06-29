using HOMMS.Domain.Dtos;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IEmailVerifyService
    {

        Task SendEmailAsync(MessageDto message);


    }
}
