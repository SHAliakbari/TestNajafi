using Najafi_Test.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Najafi_Test.Models
{
    public abstract class BaseEntity  : IBaseEntity
    {
        #region Properties

        public int Id { get; set; }
        public Guid Guid { get; set; }
        public bool? IsDisabled { get; set; }

        ///Other shared properties like CreateUser , ModifiedUser comes here

        #endregion
    }

    public abstract class BaseEntity<T> : BaseEntity, IBaseEntity<T>
        where T : BaseEntity
    {
        #region Events
        public delegate void SavingEventHandler(T sender, SavingModel e);
        public event SavingEventHandler SavingEvent;

        public delegate void SavedEventHandler(T sender);
        public event SavedEventHandler SavedEvent;

        #endregion

        #region Methods

        public abstract bool Validate(ref List<string> Errors);
        public virtual bool Saving()
        {
            if(SavingEvent != null)
            {
                var model = new SavingModel()
                {
                    Result = true
                };
                SavingEvent.Invoke((T)(object)this, model);
                
                return model.Result;
            }

            return true;
        }
        public virtual void Saved()
        {
            SavedEvent?.Invoke((T)(object)this);
        }

        #endregion

        

    }
}