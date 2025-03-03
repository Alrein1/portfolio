package config;

import dao.OrderDAO;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.io.ClassPathResource;
import org.springframework.jdbc.datasource.DriverManagerDataSource;
import org.springframework.jdbc.datasource.init.DataSourceInitializer;
import org.springframework.jdbc.datasource.init.ResourceDatabasePopulator;
import org.springframework.web.servlet.config.annotation.EnableWebMvc;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;
import service.OrderService;

import javax.sql.DataSource;
import java.sql.Driver;
import java.sql.DriverManager;


@Configuration
@EnableWebMvc
@ComponentScan(basePackages = "controller")
public class AppConfig implements WebMvcConfigurer {

    @Bean
    public DataSource dataSource() {
        DriverManagerDataSource dataSource = new DriverManagerDataSource();
        dataSource.setDriverClassName("org.hsqldb.jdbcDriver");
        dataSource.setUrl("jdbc:hsqldb:mem:db");
        dataSource.setUsername("sa");
        dataSource.setPassword("");
        // return new DriverManagerDataSource("jdbc:hsqldb:file:C:/Users/Alex/javaveeb/icd0011/src/main/db/db;ifexists=true;hsqldb.lock_file=false", "SA", "");
        return dataSource;
    }

    @Bean
    public ResourceDatabasePopulator databasePopulator() {
        ResourceDatabasePopulator populator = new ResourceDatabasePopulator();
        populator.addScript(new ClassPathResource("schema.sql"));
        return populator;
    }
    @Bean
    public DataSourceInitializer dataSourceInitializer(DataSource dataSource) {
        DataSourceInitializer initializer = new DataSourceInitializer();
        initializer.setDataSource(dataSource);
        initializer.setDatabasePopulator(databasePopulator());
        return initializer;
    }
    @Bean
    public OrderDAO orderDao(DataSource dataSource) {
        return new OrderDAO(dataSource);
    }
    @Bean
    public OrderService orderService() {
        return new OrderService();
    }
}
