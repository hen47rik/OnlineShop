CREATE TABLE IF NOT EXISTS category
(
    id   int AUTO_INCREMENT
        PRIMARY KEY,
    name varchar(40) NOT NULL
);

CREATE TABLE IF NOT EXISTS  `order`
(
    id    int AUTO_INCREMENT
        PRIMARY KEY,
    email varchar(40)                          NOT NULL,
    date  datetime DEFAULT CURRENT_TIMESTAMP NOT NULL
);

CREATE TABLE IF NOT EXISTS  product
(
    id          int AUTO_INCREMENT
        PRIMARY KEY,
    name        varchar(40)   NOT NULL,
    description varchar(100)  NULL,
    images      varchar(100)  NULL,
    amount      int DEFAULT 0 NOT NULL,
    price       int           NOT NULL
);

CREATE TABLE IF NOT EXISTS  user
(
    id          int AUTO_INCREMENT
        PRIMARY KEY,
    passwordHash blob NOT NULL,
    passwordSalt blob NOT NULL,
    email varchar(100)  NOT NULL,
    isAdmin bool NOT NULL 
);


CREATE TABLE IF NOT EXISTS  order_product
(
    `order`  int           NOT NULL,
    product  int           NOT NULL,
    quantity int DEFAULT 0 NOT NULL,
    PRIMARY KEY (`order`, product),
    CONSTRAINT order_product_order_null_fk
        FOREIGN KEY (`order`) REFERENCES `order` (id),
    CONSTRAINT order_product_product_null_fk
        FOREIGN KEY (product) REFERENCES product (id)
);

CREATE TABLE IF NOT EXISTS  product_category
(
    product  int NOT NULL,
    category int NOT NULL,
    PRIMARY KEY (product, category),
    CONSTRAINT product_category_category_null_fk
        FOREIGN KEY (category) REFERENCES category (id),
    CONSTRAINT product_category_product_null_fk
        FOREIGN KEY (product) REFERENCES product (id)
);

CREATE TABLE IF NOT EXISTS  user_order
(
    user  int NOT NULL,
    `order` int NOT NULL,
    PRIMARY KEY (user, `order`),
    CONSTRAINT user_order_user_null_fk
        FOREIGN KEY (user) REFERENCES user (id),
    CONSTRAINT user_order_order_null_fk
        FOREIGN KEY (`order`) REFERENCES `order` (id)
);


