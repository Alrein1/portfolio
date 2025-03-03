package model;

import jakarta.validation.Valid;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;

//@Entity
//@Table(name="orders')
public class Order {
    //@Id
    //@GeneratedValue
    private Integer id;
    //@Column(name="number")
    private String orderNumber;
    @Valid
    //@OneToMany
    //or @ElementCollection, orderRows @Embeddable, no ID
    private List<OrderRow> orderRows;

    public Order() {}

    public Order(String orderNumber) {
        this.orderNumber = orderNumber;
    }

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getOrderNumber() {
        return orderNumber;
    }

    public void setOrderNumber(String orderNumber) {
        this.orderNumber = orderNumber;
    }

    public List<OrderRow> getOrderRows() {
        return orderRows;
    }

    public void add(OrderRow orderRow) {
        if (orderRows == null) {
            orderRows = new ArrayList<>();
        }

        orderRows.add(orderRow);
    }

    @Override
    public boolean equals(Object o) {
        if (!(o instanceof Order)) {
            return false;
        }
        Order order = (Order) o;
        return Objects.equals(id, order.id)
                && Objects.equals(orderNumber, order.orderNumber)
                && Objects.equals(orderRows, order.orderRows);
    }

    @Override
    public int hashCode() {
        return 1;
    }

    @Override
    public String toString() {
        return "Order{" +
                "id=" + id +
                ", orderNumber='" + orderNumber + '\'' +
                ", orderRows=" + orderRows +
                '}';
    }

}
