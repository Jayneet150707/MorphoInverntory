using MorphoInventoryApi.Models;
using MorphoInventoryApi.Repositories;

namespace MorphoInventoryApi.Services
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;

        public OrderService(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<DeviceOrderRequest>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task<DeviceOrderRequest?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        public async Task<List<DeviceOrderRequest>> GetOrdersByStatusAsync(string status)
        {
            return await _orderRepository.GetOrdersByStatusAsync(status);
        }

        public async Task<int> CreateOrderAsync(CreateOrderRequest request)
        {
            var order = new DeviceOrderRequest
            {
                Creator = request.Creator,
                Branch = request.Branch,
                BranchAddress = request.BranchAddress,
                PersonName = request.PersonName,
                MobileNumber = request.MobileNumber,
                DeviceCount = request.DeviceCount,
                RequestDate = DateTime.Now,
                Status = "Pending"
            };
            
            return await _orderRepository.CreateOrderAsync(order);
        }

        public async Task<bool> ApproveOrderAsync(ApproveOrderRequest request)
        {
            var status = request.IsApproved ? "Approved" : "Rejected";
            return await _orderRepository.UpdateOrderStatusAsync(request.OrderId, status, request.ApprovedBy);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            return await _orderRepository.UpdateOrderStatusAsync(orderId, status);
        }
    }
}

