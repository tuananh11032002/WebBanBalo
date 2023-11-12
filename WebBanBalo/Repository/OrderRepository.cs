using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata.Ecma335;
using WebBanBalo.Data;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Repository
{
    public class OrderRepository:IOrderRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public OrderRepository(DataContext dataContext,IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<bool> AddOrder(Order order)
        {
            await _dataContext.Order.AddAsync(order);
           
            return await SaveAsync();
        }

        private async Task<bool> SaveAsync()
        {
            var saved = await _dataContext.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
        private bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;
        }
        public async Task<Order> FindOrderWithUserId(string userId)
        {
            var order= await _dataContext.Order.FirstOrDefaultAsync(p=>p.UserId.ToString()==userId && p.Done==false);
            return order;
        }

        public async Task< object > FindOrder(string userId)
        {

            var order = await _dataContext.Order
                .Where(p => p.UserId == Int32.Parse(userId) && p.Done == false).Select(p => new
                {
                    Id = p.Id,
                    TotalAmount = p.TotalAmount,
                    FeeShip = 30000,
                    ProductOrder = p.OrderItems.Where(pr => pr.Product.Soluong > 0).Select(or => new
                    {
                        Id = or.ProductId,
                        Image = or.Product.Images.Select(im => im.FilePath).First(),
                        ProductHas = or.Product.Soluong,
                        Price = or.Product.Price,
                        Quantity = or.Quantity,
                        Name = or.Product.Name
                    }).ToList(),
                    TotalProduct = p.OrderItems.Count,
                    Done = p.Done,
                }).FirstOrDefaultAsync(); 
            return order;
        }
        public ICollection<Order> GetOrder()
        {
            return _dataContext.Order.ToList();
        }

        public ValueReturn AddProduct(OrderItem orderItem)
        {
            var productCheck = _dataContext.Product.Where(p => p.Soluong >= orderItem.Quantity && p.Id == orderItem.Product.Id).FirstOrDefault();
            
                if (productCheck != null)
                {
                //Is Exist
                var check = _dataContext.OrderItem.Where(p => p.ProductId == orderItem.Product.Id && p.OrderId == orderItem.Order.Id).FirstOrDefault();
                var order = _dataContext.Order.Where(p => p.Id == orderItem.Order.Id).FirstOrDefault();
                //            var orderProductList = _dataContext.OrderItem.Where(p => p.OrderId == orderItem.Order.Id).ToList();

                if (check != null)
                {

                    check.Quantity = orderItem.Quantity;
                    float totalPrice = 0;
                    foreach (var t in order.OrderItems)
                    {
                        totalPrice += t.Price * t.Quantity;
                    }
                    order.TotalAmount = totalPrice;

                }
                else
                {
                    _dataContext.Add(orderItem);
                    _dataContext.SaveChanges();
                    var orderItemTemp = _dataContext.OrderItem.Where(p => p.ProductId == orderItem.Product.Id && p.OrderId == orderItem.Order.Id).FirstOrDefault();
                    var orderProductListTemp = _dataContext.OrderItem.Where(p => p.OrderId == orderItem.Order.Id).ToList();
                    orderItemTemp.Quantity = orderItem.Quantity;
                    float totalPrice = 0;
                    foreach (var t in orderProductListTemp)
                    {
                        totalPrice += t.Price * t.Quantity;
                    }
                    order.TotalAmount = totalPrice;

                }

            }
            else
            {
                return new ValueReturn()
                {
                    Status = false,
                    Message = "Số lượng sản phẩm không đủ với yêu cầu của bạn"
                };
            }
            Save();
            return new ValueReturn()
            {
                Status = true,
                Message = "Thêm thành công"
            };
        }

        public Order GetOrder(int id)
        {
            return _dataContext.Order.Where(p => p.Id == id).FirstOrDefault();
        }

        public async Task<List<OrderItemDto>> getProductOrder(int orderid)
        {
            float totalPrice = await _dataContext.OrderItem.Where(p => p.OrderId == orderid).Select(pc=>pc.Quantity*pc.Product.Price).SumAsync();
            
             List<OrderItemDto> result = await _dataContext.OrderItem.Where(p => p.OrderId == orderid).Select(p => new OrderItemDto { Product=_mapper.Map<ProductDto>(new Product { Name=p.Product.Name,Images=p.Product.Images, Id=p.Product.Id,Price=p.Product.Price }), Price=totalPrice, Quantity=p.Quantity}).ToListAsync();
            
            return result;
;
        }

        public OrderItem FindOrderItem(int productId, int orderId)
        {
            var orderItem = _dataContext.OrderItem.Where(p => p.ProductId== productId && p.OrderId==orderId ).FirstOrDefault();
            return orderItem;
        }

        public async Task<bool> Delete(OrderItem orderItem)
        {
            

             _dataContext.Remove(orderItem);
            await _dataContext.SaveChangesAsync();
            var order = _dataContext.Order.Where(p => p.Id == orderItem.OrderId).FirstOrDefault();

            var listProduct = await _dataContext.OrderItem.Where(p => p.OrderId == orderItem.OrderId).ToListAsync();

            float result = 0;

            foreach (var item in listProduct)
            {
                result += item.Price*item.Quantity;
            }
            order.TotalAmount = result;

            
            return await SaveAsync();
        }

        public async Task<ValueReturn> ConfirmOrder(OrderUpdateModel order, int userId)
        {
            try
            {
                var orderUpdate = _dataContext.Order.FirstOrDefault(p => p.Id == order.Id && p.UserId == userId && p.Done == false);
                //check quantity is valid
                var orderItem = await _dataContext.Order.Where(p => p.Id == order.Id).Select(px => new 
                {
                    OrderId = px.Id,
                    ProductList= px.OrderItems.Select(py=> new {Name= py.Product.Name, SoLuong= py.Product.Soluong,SoLuongCan= py.Quantity}).ToList(),
                    
                    
                }).FirstOrDefaultAsync();
                if (orderItem != null)
                {
                    string message = "";
                    foreach(var  item in orderItem.ProductList) {
                        if (item.SoLuongCan > item.SoLuong)
                        {
                            message = message + $"\n {item.Name} không đủ hàng so với nhu cầu của bạn ";

                        }
                    }
                    if (!message.IsNullOrEmpty())
                    {
                        return new ValueReturn()
                        {
                            Status = false,
                            Message = message
                        };
                    }
                }
                
                if (orderUpdate != null)
                {
                    orderUpdate.Discount = order.Discount;
                    orderUpdate.FinishedAt = DateTime.Now;
                    orderUpdate.ShippingAddress = order.ShippingAddress;
                    orderUpdate.OrderNote = order.OrderNote;
                    orderUpdate.FeeShip = order.FeeShip;
                    orderUpdate.BillingAddress = order.BillingAddress;
                    orderUpdate.PaymentMethod = order.PaymentMethod;
                    orderUpdate.Done = true;
                    _dataContext.Update(orderUpdate);
                    await _dataContext.SaveChangesAsync();


                    if (orderUpdate.Done)
                    {
                        var orderTemp = new Order
                        {
                            UserId = userId
                        };

                        _dataContext.Order.Add(orderTemp);

                        await _dataContext.SaveChangesAsync();
                    }

                    var orderItems = _dataContext.OrderItem.Where(oi => oi.OrderId == order.Id).ToList();

                    foreach (var oi in orderItems)
                    {
                        var product = _dataContext.Product.Find(oi.ProductId);

                        if (product != null)
                        {
                            product.Soluong -= oi.Quantity;

                            _dataContext.Product.Update(product);
                        }
                    }

                    // Lưu lại các thay đổi
                    await _dataContext.SaveChangesAsync();


                    return new ValueReturn()
                    {
                        Status = true,
                        Message = "Cập nhật thành công"

                    };
                }
                else
                {
                    return new ValueReturn
                    {
                        Status = false
                        ,
                        Message = "Có lỗi gì đó với đơn hàng"
                    };
                }


            }
            catch (Exception ex) 
            {
                return new ValueReturn
                {
                    Status = false
                       ,
                    Message = "Có lỗi gì đó với đơn hàng: " + ex 
                };
            }

        }

        public async Task<ValueReturn> getOrderDoneWithUserId(int userId, int pageIndex, int pageSize)
        {
            try
            {
                var result = _dataContext.Order.Where(p => p.UserId == userId && p.Done == true).Select(p => new
                {
                    OrderId = p.Id,
                    FeeShip = p.FeeShip,
                    Discount = p.Discount,
                    GrandTotal = p.GrandTotal,
                    TotalAmount = p.TotalAmount,
                    OrderStatus = p.OrderStatus,
                    Product = p.OrderItems.Select(o => new
                    {
                        Quantity = o.Quantity,
                        Name = o.Product.Name,
                        Id = o.ProductId,
                        Price = o.Price,
                        Image = o.Product.Images.Select(im => im.FilePath).FirstOrDefault()

                    })
                }).Skip((pageIndex-1)*pageSize).Take(pageSize).ToList() ;

                var result2 = new
                {
                    totalPage = Math.Ceiling(_dataContext.Order.Where(p => p.UserId == userId && p.Done == true).Count() / (float)pageSize),
                    data = result,
                    totalOrder = _dataContext.Order.Where(p => p.UserId == userId && p.Done == true).Count()
                };

                return new ValueReturn { Status = true, Message = "Lấy thành công", Data=result2 };

            }catch (Exception ex)
            {
                return new ValueReturn
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }
    }
}
