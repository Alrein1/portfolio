package servlet;

import com.fasterxml.jackson.databind.ObjectMapper;
import dao.OrderDAO;
import jakarta.servlet.ServletContext;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.ws.rs.core.MediaType;
import model.Order;
import model.ValidationError;
import model.ValidationErrors;
import org.springframework.jdbc.datasource.DriverManagerDataSource;
import util.Util;

import java.io.IOException;
import java.util.*;

@WebServlet({"/api/orders", "/orders/form"})
public class OrderServlet extends HttpServlet {
    private Integer index = 0;
    private OrderDAO orderDao;

    @Override
    public void init() {
        ServletContext servletContext = getServletContext();
        this.orderDao = (OrderDAO) servletContext.getAttribute("orderDao");
    }

    @Override
    public void doPost(HttpServletRequest request, HttpServletResponse response) throws IOException {
        String path = request.getServletPath();

        if ("/api/orders".equals(path)) {
            handleJsonOrderPost(request, response);
        } else if ("/orders/form".equals(path)) {
            handleFormOrderPost(request, response);
        }
    }

    private void handleJsonOrderPost(HttpServletRequest request, HttpServletResponse response) throws IOException {
        ServletContext context = getServletContext();
        Map<Integer, Map<String, Object>> orders = getOrders(context);
        String json = Util.readStream(request.getInputStream());
        ObjectMapper mapper = new ObjectMapper();
        Order order = mapper.readValue(json, Order.class);
        if (order.getOrderNumber().length() < 2) {
            response.setStatus(HttpServletResponse.SC_BAD_REQUEST);
            ValidationErrors errors = new ValidationErrors();
            errors.addError(new ValidationError("too_short_number"));
            response.setContentType("application/json");
            response.getWriter().print(mapper.writeValueAsString(errors));
            return;
        }
        Map<String, Object> orderData = mapper.readValue(json, Map.class);

        Order withId = this.orderDao.save(order);

        response.setContentType(MediaType.APPLICATION_JSON);
        response.getWriter().print(mapper.writeValueAsString(withId));
    }

    private void handleFormOrderPost(HttpServletRequest request, HttpServletResponse response) throws IOException {
        ServletContext context = getServletContext();
        Map<Integer, Map<String, Object>> orders = getOrders(context);

        String orderNumber = request.getParameter("orderNumber");


        int id = incrementIndex();
        Map<String, Object> orderData = new HashMap<>();
        orderData.put("id", id);
        orderData.put("orderNumber", orderNumber);
        orders.put(id, orderData);

        String accept = request.getHeader("Accept");
        if (MediaType.APPLICATION_JSON.equals(accept)) {
            response.setContentType(MediaType.APPLICATION_JSON);
            ObjectMapper mapper = new ObjectMapper();
            response.getWriter().print(mapper.writeValueAsString(orderData));
        } else {
            response.setContentType(MediaType.APPLICATION_FORM_URLENCODED);
            response.getWriter().print("id=" + id + "&orderNumber=" + orderNumber);
        }
    }

    @Override
    public void doGet(HttpServletRequest request, HttpServletResponse response) throws IOException {
        String idParam = request.getParameter("id");

        if (idParam == null) {
            List<Order> orders = orderDao.findAll();
            response.setContentType(MediaType.APPLICATION_JSON);
            response.getWriter().print(new ObjectMapper().writeValueAsString(orders));
            return;
        }

        Order order = orderDao.findById(Integer.parseInt(idParam));

        response.setContentType(MediaType.APPLICATION_JSON);
        response.getWriter().print(new ObjectMapper().writeValueAsString(order));
    }

    @Override
    public void doDelete(HttpServletRequest request, HttpServletResponse response) throws IOException {
        String idParam = request.getParameter("id");
        this.orderDao.delete(Integer.parseInt(idParam));
    }
    private synchronized Integer incrementIndex() {
        return ++this.index;
    }

    private Map<Integer, Map<String, Object>> getOrders(ServletContext context) {
        Map<Integer, Map<String, Object>> orders = (Map<Integer, Map<String, Object>>) context.getAttribute("orders");

        if (orders == null) {
            orders = new HashMap<>();
            context.setAttribute("orders", orders);
        }

        return orders;
    }
}
