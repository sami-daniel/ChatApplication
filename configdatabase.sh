echo "ALTER USER 'root'@'%' IDENTIFIED WITH mysql_native_password BY '${MYSQL_ROOT_PASSWORD}';" >> /host/init.sql
echo "FLUSH PRIVILEGES" >> /host/init.sql;

echo "Generated init.sql file";