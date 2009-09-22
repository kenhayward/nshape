﻿/******************************************************************************
  Copyright 2009 dataweb GmbH
  This file is part of the nShape framework.
  nShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  nShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  nShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Drawing;


namespace Dataweb.NShape.Advanced {

	public abstract class nShapeAction {

		protected nShapeAction() {
		}


		protected nShapeAction(string title)
			: this() {
			this.title = title;
		}


		protected nShapeAction(string title, Bitmap image, Color imageTransparentColor)
			: this(title) {
			this.image = image;
			this.transparentColor = imageTransparentColor;
			if (this.image != null) this.image.MakeTransparent(this.transparentColor);
		}


		protected nShapeAction(string title, Bitmap image, string description, bool isFeasible)
			: this(title) {
			this.image = image;
			this.description = description;
			this.isFeasible = isFeasible;
		}


		protected nShapeAction(string title, Bitmap image, Color transparentColor,
			string name, string description, bool isChecked, bool isFeasible)
			: this(title, image, transparentColor) {
			this.name = name;
			this.description = description;
			this.isChecked = isChecked;
			this.isFeasible = isFeasible;
		}


		public object Tag {
			get { return tag; }
			set { tag = value; }
		}


		/// <summary>
		/// Culture invariant name that can be used as key for the presenting widget.
		/// </summary>
		public virtual string Title {
			get { return title; }
			set { title = value; }
		}


		/// <summary>
		/// Culture-depending title to display as caption of the presenting widget.
		/// </summary>
		public virtual string Name {
			get {
				if (string.IsNullOrEmpty(name))
					return this.GetType().Name;
				else return name;
			}
			set { name = value; }
		}


		/// <summary>
		/// This text is displayed as tool tip by the presenting widget.
		/// Describes the performed action if active, the reason why it is disabled if the requirement for the action 
		/// is not met (e.g. Unselecting shapes requires selected shapes) or the reason why the action is not allowed.
		/// </summary>
		public virtual string Description {
			get { return description; }
			set { description = value; }
		}


		/// <summary>
		/// Subitems of the action.
		/// </summary>
		public virtual nShapeAction[] SubItems {
			get { return subItems; }
		}


		/// <summary>
		/// True if all requirements for performing the action are met. If false, the presenting widget should appear disabled.
		/// </summary>
		public virtual bool IsFeasible {
			get { return isFeasible; }
			set { isFeasible = value; }
		}


		/// <summary>
		/// Specifies if the action may or may not be executed due to security restrictions.
		/// </summary>
		public abstract bool IsGranted(ISecurityManager securityManager);


		/// <summary>
		/// True if the presenting item should appear as checked item.
		/// </summary>
		public virtual bool Checked {
			get { return isChecked; }
			set { isChecked = value; }
		}


		/// <summary>
		/// An image for the presenting widget's icon.
		/// </summary>
		public virtual Bitmap Image {
			get { return image; }
			set {
				image = value;
				if (image != null && transparentColor != Color.Empty)
					image.MakeTransparent(transparentColor);
			}
		}


		public virtual Color ImageTransparentColor {
			get { return transparentColor; }
			set {
				transparentColor = value;
				if (image != null && transparentColor != Color.Empty)
					image.MakeTransparent(transparentColor);
			}
		}


		public abstract void Execute(nShapeAction action, Project project);


		#region Fields

		protected nShapeAction[] subItems = null;

		private object tag = null;
		private string title = string.Empty;
		private string name = null;
		private string description = null;
		private bool isFeasible = true;
		private bool isChecked = false;
		private Bitmap image = null;
		private Color transparentColor = Color.Empty;

		#endregion
	}


	/// <summary>
	/// Dummy action for creating MenuSeperators
	/// </summary>
	public class SeparatorAction : nShapeAction {

		public SeparatorAction() : base() { }


		public override void Execute(nShapeAction action, Project project) {
			if (action == null) throw new ArgumentNullException("action");
			if (project == null) throw new ArgumentNullException("project");
			// nothing to do
		}


		public override string Name {
			get { return name; }
			set { /* nothing to do */ }
		}


		public override string Title {
			get { return title; }
			set { /* nothing to do */ }
		}


		public override string Description {
			get { return string.Empty; }
			set { /* nothing to do */ }
		}


		public override bool IsGranted(ISecurityManager securityManager) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			return true;
		}


		public override bool IsFeasible {
			get { return true; }
			set { /* nothing to do */ }
		}


		public override bool Checked {
			get { return false; }
			set { /* nothing to do */ }
		}


		public override Bitmap Image {
			get { return null; }
			set { /* nothing to do */ }
		}


		public override Color ImageTransparentColor {
			get { return Color.Empty; }
			set { /* nothing to do */ }
		}


		private const string name = "SeparatorAction";
		private const string title = "----------";
	}


	/// <summary>
	/// Throws a NotImplementedException. 
	/// This class is meant as a placeholder and should never be used in a productive environment.
	/// </summary>
	public class NotImplementedAction : nShapeAction {

		public NotImplementedAction(string title)
			: base(title) {
		}

		public override void Execute(nShapeAction action, Project project) {
			if (action == null) throw new ArgumentNullException("action");
			if (project == null) throw new ArgumentNullException("project");
			throw new NotImplementedException();
		}


		public override bool IsGranted(ISecurityManager securityManager) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			return true;
		}


		public override bool IsFeasible {
			get { return false; }
			set { /* nothing to do */ }
		}


		public override bool Checked {
			get { return false; }
			set { /* nothing to do */ }
		}


		public override Bitmap Image {
			get { return null; }
			set { /* nothing to do */ }
		}


		public override Color ImageTransparentColor {
			get { return Color.Empty; }
			set { /* nothing to do */ }
		}


		private const string notImplementedText = "This action is not yet implemented.";
	}


	public class nShapeActionGroup : nShapeAction {

		public nShapeActionGroup()
			: base() {
		}


		public nShapeActionGroup(string title)
			: base(title) {
		}


		public nShapeActionGroup(string title, Bitmap image, Color imageTransparentColor)
			: base(title, image, imageTransparentColor) {
		}


		public nShapeActionGroup(string title, Bitmap image, string description, bool isFeasible)
			: base(title, image, description, isFeasible) {
		}


		public nShapeActionGroup(string title, Bitmap image, Color transparentColor, string name, string description, bool isChecked, bool isFeasible)
			: base(title, image, transparentColor, name, description, isChecked, isFeasible) {
		}


		public nShapeActionGroup(string title, Bitmap image, string description, bool isFeasible, nShapeAction[] actions, int defaultActionIndex)
			: base(title, image, description, isFeasible) {
			this.subItems = actions;
			this.defaultActionIdx = defaultActionIndex;
		}


		public nShapeActionGroup(string title, Bitmap image, Color transparentColor, string name, string description, bool isChecked, bool isFeasible, nShapeAction[] actions, int defaultActionIndex)
			: base(title, image, transparentColor, name, description, isChecked, isFeasible) {
			this.subItems = actions;
			this.defaultActionIdx = defaultActionIndex;
		}


		public override bool IsGranted(ISecurityManager securityManager) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			return true;
		}


		public override void Execute(nShapeAction action, Project project) {
			//if (action == null) throw new ArgumentNullException("action");
			//if (project == null) throw new ArgumentNullException("project");
			//if (DefaultAction != null) DefaultAction.Execute(DefaultAction, project);
		}


		public nShapeAction DefaultAction {
			get { return (subItems == null || defaultActionIdx < 0 || defaultActionIdx >= subItems.Length) ? null : subItems[defaultActionIdx]; }
		}


		private int defaultActionIdx = -1;
	}


	/// <summary>
	/// Executed a given delegate.
	/// </summary>
	public class DelegateAction : nShapeAction {

		public delegate void ActionExecuteDelegate(nShapeAction action, Project project);


		public DelegateAction(string text)
			: base(text, null, Color.Empty) {
		}


		public DelegateAction(string text, Bitmap image, Color imageTransparentColor)
			: base(text, image, imageTransparentColor) {
		}


		public DelegateAction(string title, Bitmap image, string description, bool isFeasible, Permission requiredPermission, ActionExecuteDelegate executeDelegate)
			: base(title, image, Color.Fuchsia, string.Format("{0} Action", title), description, false, isFeasible) {
			this.requiredPermission = requiredPermission;
			this.executeDelegate = executeDelegate;
		}


		public DelegateAction(string title, Bitmap image, Color transparentColor, string name, string description, bool isChecked, bool isFeasible, Permission requiredPermission, ActionExecuteDelegate executeDelegate)
			: base(title, image, transparentColor, name, description, isChecked, isFeasible) {
			this.requiredPermission = requiredPermission;
			this.executeDelegate = executeDelegate;
		}


		public override void Execute(nShapeAction action, Project project) {
			if (action == null) throw new ArgumentNullException("action");
			if (project == null) throw new ArgumentNullException("project");
			executeDelegate(action, project);
		}


		public override bool IsGranted(ISecurityManager securityManager) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			return securityManager.IsGranted(requiredPermission);
		}


		public Permission RequiredPermission {
			get { return requiredPermission; }
			set { requiredPermission = value; }
		}


		public ActionExecuteDelegate Delegate {
			get { return executeDelegate; }
			set { executeDelegate = value; }
		}


		// Fields
		private Permission requiredPermission = Permission.None;
		private ActionExecuteDelegate executeDelegate;
	}


	/// <summary>
	/// Adds a Command to the History and executes it.
	/// </summary>
	public class CommandAction : nShapeAction {

		public CommandAction()
			: base() { }


		public CommandAction(string title)
			: base(title) { }


		public CommandAction(string title, Bitmap image, Color transparentColor)
			: base(title, image, transparentColor) { }


		public CommandAction(string title, Bitmap image, string notFeasibleDescription, bool isFeasible, ICommand command)
			: base(title, image, notFeasibleDescription, isFeasible) {
			this.command = command;
		}


		public CommandAction(string title, Bitmap image, Color transparentColor, string name, string notFeasibleDescription, bool isChecked, bool isFeasible, ICommand command)
			: base(title, image, transparentColor, name, notFeasibleDescription, isChecked, isFeasible) {
			this.command = command;
		}


		public override string Description {
			get {
				if (IsFeasible) return command.Description;
				else return base.Description;
			}
			set { base.Description = value; }
		}


		public override bool IsGranted(ISecurityManager securityManager) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			return (command != null) ? command.IsAllowed(securityManager) : true;
		}


		public override void Execute(nShapeAction action, Project project) {
			if (action == null) throw new ArgumentNullException("action");
			if (project == null) throw new ArgumentNullException("project");
			if (command != null) project.ExecuteCommand(command);
		}


		public ICommand Command {
			get { return command; }
		}


		#region Fields
		private ICommand command = null;
		#endregion
	}

}
