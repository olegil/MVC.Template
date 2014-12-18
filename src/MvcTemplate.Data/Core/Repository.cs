﻿using AutoMapper.QueryableExtensions;
using MvcTemplate.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace MvcTemplate.Data.Core
{
    public class Repository<TModel> : IRepository<TModel> where TModel : BaseModel
    {
        private IDbSet<TModel> repository;
        private AContext context;

        Type IQueryable.ElementType
        {
            get
            {
                return repository.ElementType;
            }
        }
        Expression IQueryable.Expression
        {
            get
            {
                return repository.Expression;
            }
        }
        IQueryProvider IQueryable.Provider
        {
            get
            {
                return repository.Provider;
            }
        }

        public Repository(AContext context)
        {
            repository = context.Set<TModel>();
            this.context = context;
        }

        public TModel GetById(String id)
        {
            return repository.SingleOrDefault(model => model.Id == id);
        }
        public TView GetById<TView>(String id) where TView : BaseView
        {
            return To<TView>().SingleOrDefault(view => view.Id == id);
        }

        public IQueryable<TView> To<TView>() where TView : BaseView
        {
            return repository.Project().To<TView>();
        }

        public void Insert(TModel model)
        {
            repository.Add(model);
        }
        public void Update(TModel model)
        {
            TModel attachedModel = repository.Local.SingleOrDefault(localModel => localModel.Id == model.Id);
            if (attachedModel == null)
                attachedModel = repository.Attach(model);
            else
                context.Entry(attachedModel).CurrentValues.SetValues(model);

            DbEntityEntry<TModel> entry = context.Entry(attachedModel);
            entry.State = EntityState.Modified;
            entry.Property(property => property.CreationDate).IsModified = false;
        }
        public void Delete(TModel model)
        {
            repository.Remove(model);
        }
        public void Delete(String id)
        {
            Delete(repository.Find(id));
        }

        public IEnumerator<TModel> GetEnumerator()
        {
            return repository.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
