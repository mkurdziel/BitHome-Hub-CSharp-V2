using System;
using System.Collections.Generic;
using BitHome.Messaging.Protocol;

namespace BitHome.Actions
{
	public class ActionBase
	{
		private const string DEFAULT_NAME = "Unknown Action";

		public String Key { get; set; }
		public String Name { get; set; }
		public String ExecutionErrorString { get; set; }
		public String Description { get; set; }
		public int NumberParameters { get; set; }
		public DataType ReturnDataType { get; set; }
		public String ReturnValue { get; set; }

		private List<IActionParameter> m_parameters = new List<IActionParameter>();
		private object m_actionLock = new object();

		public 

		public ActionBase ()
		{
		}
	}
}
//
//	/**
//	 * @return a list of parameters associated with this Action
//	 */
//	public IActionParameter[] getParameters()
//	{
//		return m_parameters.toArray(new ActionParameter[m_parameters.size()]);
//	}
//
//	/**
//	 * Set the parameter for a specific index
//	 * 
//	 * @param p_index
//	 * @param p_parameter
//	 */
//	public void addParameter(IActionParameter p_parameter)
//	{
//		Logger.v(TAG, getActionIdString() + ": adding parameter " + p_parameter.getName());
//
//		m_parameters.add(p_parameter);
//	}
//
//	/**
//	 * @param p_index
//	 * @return the parameter at the index. Returns null if none set.
//	 */
//	public IActionParameter getParameter(int p_index)
//	{
//		return m_parameters.get(p_index);
//	}
//
//	/**
//	 * Clear all the parameters for this Action
//	 */
//	public void clearParameters()
//	{
//		m_parameters.clear();
//	}
//
//	/**
//	 * Set the name of the Action
//	 * 
//	 * @param p_name
//	 */
//	public void setName(String p_name)
//	{
//		m_name = p_name;
//	}
//
//	/**
//	 * @return the name of the Action
//	 */
//	public String getName()
//	{
//		return m_name;
//	}
//
//	/**
//	 * @return the Action ID
//	 */
//	public short getActionId()
//	{
//		return m_actionId;
//	}
//
//	/**
//	 * @return the description of the Action
//	 */
//	public String getDescription()
//	{
//		return m_description;
//	}
//
//	/**
//	 * Set the number of parameters in this Action
//	 * 
//	 * @param p_numParameters
//	 */
//	public void setNumParameters(int p_numParameters)
//	{
//		m_numParameters = p_numParameters;
//
//		m_parameters = new ArrayList<IActionParameter>(p_numParameters);
//	}
//
//	/**
//	 * @return the number of parameters that should be in this function
//	 */
//	public int getNumParameters()
//	{
//		return m_numParameters;
//	}
//
//	/**
//	 * TODO: fix this!!!!!
//	 * 
//	 * @return the count of the parameter storage
//	 */
//	public int getParameterCount()
//	{
//		return m_parameters.size(); 
//	}
//
//
//	/**
//	 * @return the Action ID as a formatted string
//	 */
//	public String getActionIdString()
//	{
//		return String.format("0x%x", m_actionId);
//	}
//
//	/**
//	 * Prepare and lock the action for execution
//	 */
//	public final void prepareExecute()
//	{
//		m_executeErrorString = "";
//		// Lock so this action can only be executed once at a time
//		m_actionLock.lock();
//	}
//
//	/**
//	 * @return true if the Action executed successfully
//	 */
//	public abstract boolean execute(
//		NodeManager p_nodeManager, 
//		ActionManager p_actionManager,
//		MsgDispatcher p_msgDispatcher,
//		long p_timeoutMilliseconds);
//
//	/**
//	 * Finish and unlock the execution
//	 */
//	public final void finishExecute()
//	{
//		// Release the action to the system
//		m_actionLock.unlock();
//	}
//
//	/**
//	 * @return an optional execution error string
//	 */
//	public String getExecuteErrorString()
//	{
//		return m_executeErrorString;
//	}
//
//	/**
//	 * Set the execution error string
//	 * 
//	 * @param p_executeErrorString
//	 */
//	public void setExecuteErrorString(String p_executeErrorString)
//	{
//		m_executeErrorString = p_executeErrorString;
//	}
//
//
//	/**
//	 * @return
//	 */
//	public Element serialize()
//	{
//		Element actionElement = new Element(C_STR_XML_ACTION);
//
//		// Add the other attributes
//		Attribute idAttribute = new Attribute(C_STR_XML_ID, getActionIdString());
//		Attribute nameAttribute = new Attribute(C_STR_XML_NAME, getName());
//		Attribute descAttribute = new Attribute(C_STR_XML_DESCRIPTION, String.valueOf(getDescription()));
//		Attribute returnAttribute = new Attribute(C_STR_XML_RETURNTYPE, String.valueOf(getReturnType()));
//
//		actionElement.addAttribute(idAttribute);
//		actionElement.addAttribute(nameAttribute);
//		actionElement.addAttribute(descAttribute);
//		actionElement.addAttribute(returnAttribute);
//
//		return actionElement;
//	}
//
//	/**
//	 * Deserialize the Action from XML
//	 * 
//	 * @param p_xml
//	 * @return
//	 */
//	private boolean deserialize(Element p_xml)
//	{
//		if (p_xml == null)
//		{
//			Logger.w(TAG, "deserialization failed. Null XML");
//			return false;
//		}
//
//		if (p_xml.getLocalName().compareTo(C_STR_XML_ACTION) == 0)
//		{
//			m_actionId = XmlUtils.getXmlAttributeShort(p_xml, C_STR_XML_ID);
//			if (m_actionId != 0)
//			{
//				m_name = XmlUtils.getXmlAttributeString(p_xml, C_STR_XML_NAME);
//				m_description = XmlUtils.getXmlAttributeString(p_xml, C_STR_XML_DESCRIPTION);
//				m_returnType = XmlUtils.getXmlAttributeDataType(p_xml, C_STR_XML_RETURNTYPE);
//
//				return true;
//			}
//			else
//			{
//				Logger.w(TAG, "deserialization failed. Invalid ID");
//				return false;
//			}
//		}
//		else
//		{
//			Logger.w(TAG, "deserialization failed. Incorrect root node: " + p_xml.getLocalName());
//		}
//
//		return false;
//	}
//
//	/* (non-Javadoc)
//	 * @see synet.controller.actions.IAction#isEqualTo(synet.controller.actions.IAction, boolean)
//	 */
//	public boolean isEqualTo(IAction p_action, boolean p_compareId)
//	{
//		return isEqualTo((ActionBase)p_action, p_compareId);
//	}
//
//	/**
//	 * @param p_action
//	 * @param p_compareId
//	 * @return
//	 */
//	public boolean isEqualTo(ActionBase p_action, boolean p_compareId) 
//	{
//		boolean retVal = true;
//
//		/*
//	     *     private short m_actionId;
//    private String m_name = STR_NAME;
//    private String m_executeErrorString;
//    private String m_description = "";
//    private int m_numParameters = 0;
//    private ArrayList<ActionParameter> m_parameters = new ArrayList<ActionParameter>();
//    private Lock m_actionLock = new ReentrantLock(true);
