using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookAdventure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyEntityStatusTo2States : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Actualizar los valores de EntityStatus de 3 estados a 2 estados
            // Cambiar Inactive (2) a Deleted (2) y Deleted (3) a Deleted (2)
            
            // Para todas las entidades que tienen Status = 2 (antes Inactive), cambiar a 2 (ahora Deleted)
            // Para todas las entidades que tienen Status = 3 (antes Deleted), cambiar a 2 (ahora Deleted)
            
            migrationBuilder.Sql("UPDATE [BookAdventure].[Book] SET Status = 2 WHERE Status IN (2, 3)");
            migrationBuilder.Sql("UPDATE [BookAdventure].[Customer] SET Status = 2 WHERE Status IN (2, 3)");
            migrationBuilder.Sql("UPDATE [BookAdventure].[Genre] SET Status = 2 WHERE Status IN (2, 3)");
            migrationBuilder.Sql("UPDATE [BookAdventure].[RentalOrder] SET Status = 2 WHERE Status IN (2, 3)");
            migrationBuilder.Sql("UPDATE [BookAdventure].[RentalOrderDetail] SET Status = 2 WHERE Status IN (2, 3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nota: La reversión de esta migración no es posible de manera exacta
            // porque perdemos la diferencia entre 'Inactive' y 'Deleted' del sistema anterior
            // Todas las entidades con Status = 2 (Deleted) se cambiarán a Inactive
            
            migrationBuilder.Sql("UPDATE [BookAdventure].[Book] SET Status = 2 WHERE Status = 2");
            migrationBuilder.Sql("UPDATE [BookAdventure].[Customer] SET Status = 2 WHERE Status = 2");
            migrationBuilder.Sql("UPDATE [BookAdventure].[Genre] SET Status = 2 WHERE Status = 2");
            migrationBuilder.Sql("UPDATE [BookAdventure].[RentalOrder] SET Status = 2 WHERE Status = 2");
            migrationBuilder.Sql("UPDATE [BookAdventure].[RentalOrderDetail] SET Status = 2 WHERE Status = 2");
        }
    }
}
