DROP TABLE IF EXISTS public.order_rows;
DROP TABLE IF EXISTS public.orders;

CREATE SCHEMA IF NOT EXISTS public;


CREATE TABLE IF NOT EXISTS public.orders (
                                             id INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                                             order_number VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS public.order_rows (
                                                 id INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                                                 item_name VARCHAR(255) NOT NULL,
                                                 quantity INT NOT NULL,
                                                 price DOUBLE PRECISION NOT NULL,
                                                 order_id INT,
                                                 FOREIGN KEY (order_id) REFERENCES public.orders(id)
);
