package service;

import dao.OrderDAO;
import model.Order;
import model.OrderRow;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class OrderService {

    @Autowired
    private OrderDAO orderDao;

    public Order save(Order order) {
        return orderDao.save(order);
    }

    public List<Order> findAll() {
        return orderDao.findAll();
    }

    public Order findById(Integer id) {
        return orderDao.findById(id);
    }

    public boolean delete(Integer id) {
        try {
            orderDao.delete(id);
            return true;
        } catch (Exception e) {
            return false;
        }
    }
}