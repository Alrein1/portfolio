package model;


import jakarta.validation.constraints.Min;
import java.util.Objects;

public class OrderRow {
    private String itemName;
    @Min(value = 1, message = "Quantity must be greater than zero")
    private Integer quantity;

    @Min(value = 1, message = "Price must be greater than zero")
    private Double price;

    public OrderRow() {}

    public OrderRow(String itemName, Integer quantity, Double price) {
        this.itemName = itemName;
        this.quantity = quantity;
        this.price = price;
    }

    public String getItemName() {
        return itemName;
    }

    public Integer getQuantity() {
        return quantity;
    }

    public Double getPrice() {
        return price;
    }

    @Override
    public String toString() {
        return "OrderRow{" +
                "itemName='" + itemName + '\'' +
                ", quantity=" + quantity +
                ", price=" + price +
                '}';
    }


    @Override
    public boolean equals(Object o) {
        if (!(o instanceof OrderRow)) {
            return false;
        }
        OrderRow orderRow = (OrderRow) o;
        return Objects.equals(itemName, orderRow.itemName)
                && Objects.equals(quantity, orderRow.quantity)
                && Objects.equals(price, orderRow.price);
    }

    @Override
    public int hashCode() {
        return 1;
    }
}
