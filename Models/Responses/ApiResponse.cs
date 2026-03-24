namespace JuegosTorneosApi.Models.Responses;

public class ApiResponse<T>
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public T? Datos { get; set; }
    public List<string>? Errores { get; set; }

    public static ApiResponse<T> Success(T datos, string mensaje = "Operación exitosa")
    {
        return new ApiResponse<T>
        {
            Exito = true,
            Mensaje = mensaje,
            Datos = datos,
            Errores = null
        };
    }

    public static ApiResponse<T> Fail(string mensaje, List<string>? errores = null)
    {
        return new ApiResponse<T>
        {
            Exito = false,
            Mensaje = mensaje,
            Datos = default,
            Errores = errores
        };
    }
}
