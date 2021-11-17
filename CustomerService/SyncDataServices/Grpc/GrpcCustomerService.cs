using System.Threading.Tasks;
using AutoMapper;
using CustomerService.Data;
using CustomerService.Models;
using Grpc.Core;

namespace CustomerService.SyncDataServices.Grpc
{
    public class GrpcCustomerService : GrpcCustomer.GrpcCustomerBase
    {
        private readonly ICustomerRepo<Customer> _repository;
        private readonly IMapper _mapper;

        public GrpcCustomerService(ICustomerRepo<Customer> repository, IMapper mapper)
        {
            _repository=repository;
            _mapper=mapper;
        }

        public override Task<CustomerResponse> GetAllCustomers(GetAllRequests requests, ServerCallContext context)
        {
            var response = new CustomerResponse();
            var customers = _repository.GetAllCustomers();

            foreach(var customer in customers)
            {
                response.Customer.Add(_mapper.Map<GrpcCustomerModel>(customer));
            }

            return Task.FromResult(response);
        }
    }
}