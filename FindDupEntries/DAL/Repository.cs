using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateAdvisorMatch.DAL
{
    public interface IRepository<T> where T : class
    {
        T Add(T entity);
        void AddRange(IEnumerable<T> entity);
        
        void Edit(T entity);
        Guid Delete(T entity);
        T Get(object primaryKey);

        IQueryable<T> GetAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

    }

    public interface IRepository
    {
        IQueryable<TResult> ExecuteCommand<TResult>(string query, params object[] args);
        int ExecuteCommand(string query, params object[] args);
    }

    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;
        internal DbSet<T> dbSet;

        public IUnitOfWork UnitOfWork { get { return _unitOfWork; } }
        internal DbContext Database { get { return _unitOfWork.Db; } }


        public GenericRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            this.dbSet = _unitOfWork.Db.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return dbSet.AsEnumerable().AsQueryable();
        }

        public IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = dbSet.Where(predicate);

            return query;
        }

        public virtual T Add(T entity)
        {
            dynamic obj = dbSet.Add(entity);
            this._unitOfWork.Db.SaveChanges();
            //this._unitOfWork.Db.sa
            
            return obj;
        }
        public virtual void AddRange(IEnumerable<T> entities)
        {
            
            dynamic obj = dbSet.AddRange(entities);
            this._unitOfWork.Db.SaveChanges();
        }

     
        public virtual Guid Delete(T entity)
        {
            if (_unitOfWork.Db.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dynamic obj = dbSet.Remove(entity);
            this._unitOfWork.Db.SaveChanges();
            return obj.Id;
        }

        public virtual void Edit(T entity)
        {
            dbSet.Attach(entity);
            _unitOfWork.Db.Entry(entity).State = EntityState.Modified;
            this._unitOfWork.Db.SaveChanges();
        }


        public T Get(object primaryKey)
        {
            var dbResult = dbSet.Find(primaryKey);
            return dbResult;
        }

        public IQueryable<TResult> ExecuteCommand<TResult>(string query, params object[] args)
        {
            var blogNames = Database.Database.SqlQuery<TResult>(query, args);
            return blogNames.AsQueryable();
        }

        public bool Exists(object primaryKey)
        {
            return dbSet.Find(primaryKey) == null ? false : true;
        }

        
    }

    public class GenericRepository : IRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public IUnitOfWork UnitOfWork { get { return _unitOfWork; } }
        internal DbContext Database { get { return _unitOfWork.Db; } }


        public GenericRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        public IQueryable<TResult> ExecuteCommand<TResult>(string query, params object[] args)
        {
            var blogNames = Database.Database.SqlQuery<TResult>(query, args);
            return blogNames.AsQueryable();
        }
        public int ExecuteCommand(string query, params object[] args)
        {
            return Database.Database.ExecuteSqlCommand(query, args);
        }


    }
}
