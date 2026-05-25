namespace instagram.DTOs;

public record PostReadDto(
    int Id,
    string Descriere,
    string ImageUrl,
    DateTime DataPublicarii,
    string NumeAutor 
);