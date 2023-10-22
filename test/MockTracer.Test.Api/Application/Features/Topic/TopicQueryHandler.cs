using System.Reflection.Emit;
using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MockTracer.Test.Api.Domain;
using MockTracer.Test.Api.Infrastracture.Database;
using AgileObjects.NetStandardPolyfills;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicAssemblyExample")]

namespace MockTracer.Test.Api.Application.Features.Topic;

public class TopicQueryHandler : IRequestHandler<TopicQuery, List<TopicDto>>
{
  private readonly IBlogDbContext _context;
  public TopicQueryHandler(IBlogDbContext context)
  {
    _context = context;
  }

  public async Task<List<TopicDto>> Handle(TopicQuery request, CancellationToken cancellationToken)
  {
    var db = _context as DbContext;
    var result = await (from t in _context.Topics.Where(w => w.Id > 0).OfType<Domain.Topic>().AsTracking(QueryTrackingBehavior.TrackAll).Select(s => new Topic2
    {
      Id = s.Id,
      Title = s.Title,
      Content = s.Content,
      Created = s.Created,
      Author = s.Author,
      AuthorId = s.AuthorId,
      Comments = s.Comments
    }).Select(s => (Domain.Topic)s)
                        join u in _context.Users.OfType<User>() on t.AuthorId equals u.Id
                        select new TopicDto { Id = t.Id, Title = t.Title, AuthorName = u.Nick }).ToListAsync(cancellationToken);
    /* 
    //normic
    result = await (from t in _context.Topics
                    join u in _context.Users on t.AuthorId equals u.Id
                    select (TopicDto)new VTopicDto { Id = t.Id, Title = t.Title, AuthorName = u.Nick, t1 = u, t2 = t }).ToListAsync(cancellationToken);
    var entity4 = (_context as DbContext).ChangeTracker.Entries().Select(s => s.Entity).ToList();
    */
    var childT = MyTypeBuilder.CreateNewObject<TopicDto2>();
    var tt = childT.GetType();
    // https://stackoverflow.com/questions/3862226/how-to-dynamically-create-a-class
    return result;
  }

  public class VTopicDto : TopicDto
  {
    internal User t1 { get; set; }
    internal Domain.Topic t2 { get; set; }
  }

  public class Topic2 : Domain.Topic
  {

  }

  public static class MyTypeBuilder
  {
    static int Count = 1;
    public static T CreateNewObject<T>() where T : class, new()
    {
      try
      {
        var myType = CompileResultType<T>();
        return (T)Activator.CreateInstance(myType);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        throw;
      }

    }
    public static Type CompileResultType<T>() where T : class, new()
    {
      TypeBuilder tb = GetTypeBuilder<T>();
      //ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

      // NOTE: assuming your list contains Field objects with fields FieldName(string) and FieldType(Type)
      /*foreach (var field in yourListOfFields)
        CreateProperty(tb, field.FieldName, field.FieldType);*/

      Type objectType = tb.CreateType();
      return objectType;
    }

    private static TypeBuilder GetTypeBuilder<T>() where T : class, new()
    {
      var parentType = typeof(T);
      AssemblyName aName = new AssemblyName("DynamicAssemblyExample");
      AssemblyBuilder ab =
          AssemblyBuilder.DefineDynamicAssembly(
              aName, //parentType.Assembly.GetName(),
              AssemblyBuilderAccess.Run);

      // The module name is usually the same as the assembly name.
      ModuleBuilder mb =
          ab.DefineDynamicModule(aName.Name);
      TypeBuilder tb = mb.DefineType(parentType.Name + Count++, TypeAttributes.NotPublic, typeof(T));

      return tb;
    }

    private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
    {
      FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

      PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
      MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
      ILGenerator getIl = getPropMthdBldr.GetILGenerator();

      getIl.Emit(OpCodes.Ldarg_0);
      getIl.Emit(OpCodes.Ldfld, fieldBuilder);
      getIl.Emit(OpCodes.Ret);

      MethodBuilder setPropMthdBldr =
          tb.DefineMethod("set_" + propertyName,
            MethodAttributes.Public |
            MethodAttributes.SpecialName |
            MethodAttributes.HideBySig,
            null, new[] { propertyType });

      ILGenerator setIl = setPropMthdBldr.GetILGenerator();
      Label modifyProperty = setIl.DefineLabel();
      Label exitSet = setIl.DefineLabel();

      setIl.MarkLabel(modifyProperty);
      setIl.Emit(OpCodes.Ldarg_0);
      setIl.Emit(OpCodes.Ldarg_1);
      setIl.Emit(OpCodes.Stfld, fieldBuilder);

      setIl.Emit(OpCodes.Nop);
      setIl.MarkLabel(exitSet);
      setIl.Emit(OpCodes.Ret);

      propertyBuilder.SetGetMethod(getPropMthdBldr);
      propertyBuilder.SetSetMethod(setPropMthdBldr);
    }
  }
}
