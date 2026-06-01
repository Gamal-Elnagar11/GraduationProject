 
    
global  using E_Commerce_API.Controllers;
global using E_Commerce_API.Data;
global using E_Commerce_API.Mapping;
global using E_Commerce_API.Models;
global using E_Commerce_API.OpenAPI;
global using E_Commerce_API.Reposatory.Implementation;
global using E_Commerce_API.Reposatory.Interface;
global using E_Commerce_API.Service.Implementation;
global using E_Commerce_API.Service.Interface;
global using E_Commerce_API.Static;
global using E_Commerce_API.UnitOfWork;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using System.IdentityModel.Tokens.Jwt;
global using System.Text;
global using System.Text.Json.Serialization;

global using System.ComponentModel.DataAnnotations;
global using AutoMapper;
global using E_Commerce_API.DTO.Identity;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Routing;

global using E_Commerce_API.DTO.CartDTO;
global using E_Commerce_API.DTO.CategoryDTO;
global using E_Commerce_API.DTO.FAQDTO;
global using System.Security.Claims;
global using E_Commerce_API.DTO.FeedBackDTO;

global using E_Commerce_API.DTO.ProductDTO;
global using E_Commerce_API.DTO.OrderDTO;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

global using System.ComponentModel.DataAnnotations.Schema;

global using Microsoft.AspNetCore.Mvc.Controllers;
global using Swashbuckle.AspNetCore.SwaggerGen;

global using E_Commerce_API.GenaricRepo;
global using Microsoft.EntityFrameworkCore.Storage;
