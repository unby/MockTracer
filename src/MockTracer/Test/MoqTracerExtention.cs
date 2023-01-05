using System.Data.Common;
using System.Data;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Reflection;
using System.Text.Json;
using Apps72.Dev.Data.DbMocker;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MockTracer.Test;
using Moq;
using System.Collections.Concurrent;

namespace MockTracer.Test;

public static class MockTracerExtention
{
  private static ConcurrentDictionary<Type, Type> _typeCash = new ConcurrentDictionary<Type, Type>();

  public static HttpClient GetHttpClient(this IHost testServer, Action<HttpClient>? configureHttpClient = null)
  {
    var client = testServer.GetTestClient();
    configureHttpClient?.Invoke(client);

    return client;
  }

  public static HttpClient GetHttpClient(this IWebHost testServer, Action<HttpClient>? configureHttpClient = null)
  {
    var client = testServer.GetTestClient();
    configureHttpClient?.Invoke(client);

    return client;
  }

  public static T? ReadJson<T>(this HttpResponseMessage response)
      where T : class
  {
    var responseString = response.Content.ReadAsStringAsync().Result;
    var obj = JsonSerializer.Deserialize<T>(
      responseString,
      new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });
    return obj;
  }

  public static byte[] ToUtf8Bytes(this object response)
  {
    return JsonSerializer.SerializeToUtf8Bytes(response);
  }

  /// <summary>
  /// Сериализует объект в HttpContent
  /// </summary>
  /// <param name="data">исходный объект</param>
  /// <returns>готовый HttpContent объекта data</returns>
  public static HttpContent ToHttpContent(this object data)
  {
    var content = JsonSerializer.Serialize(data);
    var buffer = System.Text.Encoding.UTF8.GetBytes(content);
    var byteContent = new ByteArrayContent(buffer);
    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    return byteContent;
  }

  public static T GetInstance<T>(this IHost host)
  {
    var result = host.Services.GetService<T>();
    return result ?? throw new Exception($"{typeof(T)} doesn't find");
  }

  public static T GetInstance<T>(this IWebHost host)
  {
    var result = host.Services.GetService<T>();
    return result ?? throw new Exception($"{typeof(T)} doesn't find");
  }

  public static IServiceCollection Replace<T>(this IServiceCollection services, Mock<T> moq, ServiceLifetime lifetime = ServiceLifetime.Scoped)
      where T : class
  {
    var desriptor = new ServiceDescriptor(typeof(T), (s) => moq.Object, lifetime);
    services.Replace(desriptor);
    return services;
  }

  public static IServiceCollection Replace<T>(this IServiceCollection services, object instance, ServiceLifetime lifetime = ServiceLifetime.Scoped)
      where T : class
  {
    var desriptor = new ServiceDescriptor(typeof(T), (s) => instance, lifetime);
    services.Replace(desriptor);
    return services;
  }

  /// <summary>
  /// Register test dbConnection provider
  /// </summary>
  /// <typeparam name="T">dbConnection provider</typeparam>
  /// <param name="services"></param>
  /// <param name="dbConnection"><see cref="MockDbConnection"/></param>
  /// <param name="lifetime"><see cref="ServiceLifetime"/></param>
  /// <returns><see cref="IServiceCollection"/></returns>
  public static IServiceCollection SetTestDBConnectionProvider<T>(this IServiceCollection services, MockDbConnection dbConnection, ServiceLifetime lifetime = ServiceLifetime.Scoped)
  {
    var type = GenerateDbProvider<T>();
    var desriptor = new ServiceDescriptor(typeof(T), (s) => (T)Activator.CreateInstance(type, dbConnection), lifetime);
    services.Replace(desriptor);
    return services;
  }

  /// <summary>
  /// Create type with logic of dbConnection provider
  /// </summary>
  /// <typeparam name="T">interface { DbConnection Get() ;} </typeparam>
  /// <returns>class extented T</returns>
  /// <exception cref="ArgumentException"></exception>
  public static Type GenerateDbProvider<T>()
  {
    var @interface = typeof(T);
    if (!@interface.IsInterface)
    {
      throw new ArgumentException("T type must be interface");
    }

    if (_typeCash.TryGetValue(@interface, out var cash))
    {
      return cash;
    }
    else
    {
      var methods = @interface.GetMethods().Where(w => w.GetParameters().Length == 0
        && (w.ReturnType == typeof(IDbConnection) || w.ReturnType == typeof(DbConnection))).ToArray();

      if (!methods.Any() || methods.Length != @interface.GetMethods().Length)
      {
        throw new ArgumentException("T type must have only the methods without arguments and out type is IDbConnection (or DbConnection)");
      }

      AssemblyName assemblyName = new AssemblyName("MockTracker.Test.Runtime");
      AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
      ModuleBuilder mb = ab.DefineDynamicModule(assemblyName.Name);

      TypeBuilder tb = mb.DefineType(
          "MockConnectionProvider",
           TypeAttributes.Public);
      tb.AddInterfaceImplementation(@interface);

      FieldBuilder mockDbConnectionField = tb.DefineField(
          "_mockDbConnection",
          typeof(IDbConnection),
          FieldAttributes.Private);

      ConstructorBuilder ctor1 = tb.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { typeof(MockDbConnection) });
      ILGenerator ctor1IL = ctor1.GetILGenerator();
      ctor1IL.Emit(OpCodes.Ldarg_0);
      ctor1IL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
      ctor1IL.Emit(OpCodes.Ldarg_0);
      ctor1IL.Emit(OpCodes.Ldarg_1);
      ctor1IL.Emit(OpCodes.Stfld, mockDbConnectionField);
      ctor1IL.Emit(OpCodes.Ret);

      foreach (var methodInfo in methods)
      {
        MethodBuilder method = tb.DefineMethod(
              @interface.Name + "." + methodInfo.Name,
              MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final,
              methodInfo.ReturnType,
              Type.EmptyTypes);

        ILGenerator methIL = method.GetILGenerator();
        methIL.Emit(OpCodes.Ldarg_0);
        methIL.Emit(OpCodes.Ldfld, mockDbConnectionField);
        methIL.Emit(OpCodes.Ret);
        tb.DefineMethodOverride(method, methodInfo);
      }

      cash = tb.CreateType();
      _typeCash.TryAdd(@interface, cash);

      return cash;
    }
  }
}
