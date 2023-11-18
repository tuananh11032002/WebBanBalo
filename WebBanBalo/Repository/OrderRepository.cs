using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System;
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
            try
            {
                await _dataContext.Order.AddAsync(order);

                return await SaveAsync();
            }catch 
            {
                return false;
            }
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
            try
            {
                var order = await _dataContext.Order.FirstOrDefaultAsync(p => p.UserId.ToString() == userId && p.Done == false);
                return order;
            }
            catch
            {
                return null;
            }

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
        public async Task<ValueReturn> GetOrder(int pageIndex, int pageSize, string? search)
        {

            try
            {
                var listOrder = _dataContext.Order.Where(p => p.Done == true).AsQueryable();

                if (!search.IsNullOrEmpty())
                {
                    search = search.ToLower();
                    listOrder = listOrder.Where(p => p.User.UserName.ToLower().Contains(search) || 
                    p.Id.ToString().Contains(search)||p.CustomerName.ToLower().Contains(search));
                }
                var result = await listOrder.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                    .Select(p => new
                    {
                        CustomerImage = p.User.Image,
                        OrderId = p.Id,
                        Date = p.FinishedAt,
                        CustomerName = p.CustomerName.IsNullOrEmpty() ? "Chưa có" : p.CustomerName,
                        CustomerEmail = "Chưa có",
                        Payment = p.PaymentStatus,
                        Status = p.OrderStatusUpdates.OrderByDescending(pc => pc.UpdateTime).Select(pc=>pc.Status).FirstOrDefault(),
                        MethodPayment = p.PaymentMethod,
                        ImagePaymentMethod = p.PaymentMethod == PaymentMethod.Momo ? "images\\MOMO.png" : "images\\COD.jpg"
                    }).ToListAsync();

                var result2 = new
                {
                    TotalOrder = listOrder.ToList().Count,
                    OrderList = result,
                };
                if (result != null)
                {
                    return new ValueReturn { Data = result2, Status = true };

                }
                else
                {
                    return new ValueReturn
                    {
                        Status = false,
                        Message = "Có vấn đề gì đó khi lấy giá trị"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ValueReturn
                {
                    Status = false,
                    Message = ex.Message,

                };
            }
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
                    orderUpdate.OrderStatusUpdates.Add(
                        new OrderStatusUpdate
                        {
                            Status = OrderStatus.ReadytoPickup,
                            UpdateTime=DateTime.Now
                        }
                        );
                    orderUpdate.CustomerPhone = order.CustomerPhone;
                    orderUpdate.CustomerName = order.CustomerName;
                    _dataContext.Update(orderUpdate);
                    await _dataContext.SaveChangesAsync();
                    if (order.PaymentMethod == PaymentMethod.Momo)
                    {
                        orderUpdate.PaymentStatus = PaymentStatus.Paid;

                    }
                    else
                    {
                        orderUpdate.PaymentStatus = PaymentStatus.Pending;

                    }

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
                    OrderStatus = p.OrderStatusUpdates.OrderByDescending(pc=>pc.UpdateTime).Select(pc=>pc.Status).FirstOrDefault(),
                    Product = p.OrderItems.Select(o => new
                    {
                        Quantity = o.Quantity,
                        Name = o.Product.Name,
                        Id = o.ProductId,
                        Price = o.Price,
                        Image = o.Product.Images.Select(im => im.FilePath).FirstOrDefault(),
                        IsReview = o.IsReview,


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

        public async Task<ValueReturn> DeleteWithOrderId(int orderId)
        {
            try
            {
                var order = await _dataContext.Order.Where(p => p.Id == orderId).FirstOrDefaultAsync();
                if(order != null)
                {
                    _dataContext.Order.Remove(order);
                    await _dataContext.SaveChangesAsync();
                    return new ValueReturn
                    {
                        Message = "Xóa thành công",
                        Status = true
                    };
                }
                else
                {
                    return new ValueReturn
                    {
                        Message = "Không tìm thấy đơn hàng",
                        Status = false

                    };
                }
            }
            catch (Exception ex)
            {
                return new ValueReturn { Status = false, Message = ex.Message };
            }
        }

        public async Task<ValueReturn> GetOrderDetailAndCustomerInfo(int orderId)
        {
            try
            {
                var order = await _dataContext.Order.Where(or => or.Id == orderId).Select(p => new
                {
                    OrderId = p.Id,
                    TotalMoney = p.TotalAmount + p.FeeShip - p.Discount,
                    Subtotal = p.TotalAmount,
                    Discount = p.Discount,
                    FeeShip = p.FeeShip,
                    OrderStatus = p.OrderStatusUpdates.Select(it => new
                    {
                        Status = it.Status,
                        DateUpdate = it.UpdateTime
                    }).OrderByDescending(it => it.DateUpdate).ToList(),
                    OrderItems = p.OrderItems.Select(it => new
                    {
                        ProductId = it.ProductId,
                        Image = it.Product.Images.Select(img => img.FilePath).FirstOrDefault(),
                        Name = it.Product.Name,
                        Price = it.Price,
                        Quantity = it.Quantity,
                        Total = it.Quantity*it.Price
                    }).ToList(),
                    UserInfor = new
                    {
                        Image = p.User.Image,
                        UserId = p.UserId,
                        DisplayName = p.User.HoTen,
                        TotalOrder = p.User.Orders.Count(),
                    },
                    CustomerInfo = new
                    {
                        Email = "Chưa có",
                        Contact = p.CustomerPhone,
                        UserNameReceive = p.CustomerName,
                        ShippingAdress = p.ShippingAddress,
                        Payment = p.PaymentMethod,
                    }
                }).FirstOrDefaultAsync();
                if(order != null )
                {
                    return new ValueReturn
                    {
                        Data= order,
                        Status = true

                    };
                }
                else
                {
                    return new ValueReturn {
                        Message = "Không tìm thấy order",
                        Status= false
                    
                    };

                }
            }
            catch (Exception ex)
            {
                return new ValueReturn { Status = false, Message = ex.Message };
            }
        }

        public async Task<ValueReturn> UpdateStatus(int orderId)
        {
            try
            {
                var order = await _dataContext.Order.Include(p=>p.OrderStatusUpdates).FirstOrDefaultAsync(p => p.Id == orderId);
                if (order != null)
                {
                    var status = order.OrderStatusUpdates.OrderByDescending(p => p.UpdateTime).FirstOrDefault();

                    int currentStatusValue = (int)status.Status;
                    int nextStatusValue = currentStatusValue + 1;

                    if (nextStatusValue <= Enum.GetValues(typeof(OrderStatus)).Length - 1)
                    {

                        var orderStatus = new OrderStatusUpdate
                        {
                            OrderId= orderId,
                            Status= (OrderStatus)nextStatusValue,
                            UpdateTime = DateTime.Now,
                        };
                         _dataContext.OrderStatusUpdates.Add(orderStatus);
                        await _dataContext.SaveChangesAsync();

                    }
                  
                    return new ValueReturn
                    {
                        Status=true,
                    };
                }
                else
                {
                    return new ValueReturn
                    {
                        Status = false,
                        Message = "Không tìm thấy order"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ValueReturn { Status = false, Message = ex.Message };
            }
        }
    }
}
