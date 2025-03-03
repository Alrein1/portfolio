package dao;

import model.Order;
import model.OrderRow;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.core.simple.JdbcClient;
import org.springframework.jdbc.core.simple.SimpleJdbcInsert;

import javax.sql.DataSource;
import java.sql.SQLException;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
//@Repository
public class OrderDAO {
    //@PersistenceContext
    // private EntityManager em;
    //bean EntityManagerFactory, factory = new LocalContainer(em)Factory, setDataSource jne, return factory.getObject;
    // properties - "hibernate.hbm2ddl.auto", "update", "create", "validate" jne. factory.setJpaProperties()
    private final JdbcTemplate jdbcTemplate;
    private final SimpleJdbcInsert orderInsert;
    private final SimpleJdbcInsert orderRowInsert;

    public OrderDAO(DataSource dataSource) {
        this.jdbcTemplate = new JdbcTemplate(dataSource);
        this.orderInsert = new SimpleJdbcInsert(dataSource)
                .withTableName("orders")
                .usingGeneratedKeyColumns("id");
        this.orderRowInsert = new SimpleJdbcInsert(dataSource)
                .withTableName("order_rows")
                .usingGeneratedKeyColumns("id");
    }
    //@Transactional
    //em.persist - insert, em.merge - update, em.remove - delete, em.createquery, :var, q.setparameter("var", var);, q.getResultStream()
    public Order save(Order order) {
        Map<String, Object> orderParams = new HashMap<>();
        orderParams.put("order_number", order.getOrderNumber());
        Number orderId = orderInsert.executeAndReturnKey(orderParams);

        if (order.getOrderRows() != null) {
            for (OrderRow row : order.getOrderRows()) {
                Map<String, Object> rowParams = new HashMap<>();
                rowParams.put("item_name", row.getItemName());
                rowParams.put("quantity", row.getQuantity());
                rowParams.put("price", row.getPrice());
                rowParams.put("order_id", orderId);
                orderRowInsert.execute(rowParams);
            }
        }
        Integer id = orderId.intValue();
        order.setId(id);
        return order;
    }

    public Order findById(Integer id) {
        String orderSql = "SELECT id, order_number FROM orders WHERE id = ?";
        Order order = jdbcTemplate.queryForObject(orderSql, new Object[]{id}, (rs, rowNum) -> {
            Order o = new Order();
            o.setId(rs.getInt("id"));
            o.setOrderNumber(rs.getString("order_number"));
            return o;
        });

        String rowSql = "SELECT id, item_name, quantity, price FROM order_rows WHERE order_id = ?";
        List<OrderRow> rows = jdbcTemplate.query(rowSql, new Object[]{id}, (rs, rowNum) -> new OrderRow(
                rs.getString("item_name"),
                rs.getInt("quantity"),
                rs.getDouble("price")
        ));

        if (order != null) {
            rows.forEach(order::add);
        }

        return order;
    }

    public List<Order> findAll() {
        String sql = "SELECT o.id AS order_id, o.order_number, r.id AS row_id, r.item_name, r.quantity, r.price " +
                "FROM orders o LEFT JOIN order_rows r ON o.id = r.order_id";

        Map<Integer, Order> orderMap = new HashMap<>();

        jdbcTemplate.query(sql, rs -> {
            int orderId = rs.getInt("order_id");
            Order order = orderMap.computeIfAbsent(orderId, id -> {
                Order newOrder = new Order();
                newOrder.setId(orderId);
                try {
                    newOrder.setOrderNumber(rs.getString("order_number"));
                } catch (SQLException e) {
                    throw new RuntimeException(e);
                }
                return newOrder;
            });
            if (rs.getInt("row_id") >= 0) {
                OrderRow row = new OrderRow(
                        rs.getString("item_name"),
                        rs.getInt("quantity"),
                        rs.getDouble("price")
                );
                order.add(row);
            }
        });

        return List.copyOf(orderMap.values());
    }

    public void delete(Integer id) {
        jdbcTemplate.update("DELETE FROM order_rows WHERE order_id = ?", id);
        jdbcTemplate.update("DELETE FROM orders WHERE id = ?", id);
    }
}
