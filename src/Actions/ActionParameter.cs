using System;
using BitHome.Messaging.Protocol;
using System.Collections.Generic;

namespace BitHome.Actions
{
	[Serializable]
	public class ActionParameter : ParameterBase, IActionParameter
	{
		public ActionParameterType ParameterType { get; set; }
		public String ActionId { get; set; }

		public ActionParameter(
			String id,
			String name,
			DataType dataType,
			ParamValidationType validationType,
			Int64 minValue,
			Int64 maxValue,
			Dictionary<String, int> enumValues,
			String actionId ) :
			base (id, name, dataType, validationType, minValue, maxValue, enumValues)
		{
			ActionId = actionId;
			ParameterType = ActionParameterType.Input;
		}

	}
}
//	/* (non-Javadoc)
//     * @see synet.controller.ParameterBase#getStrValue()
//     */
//	@Override
//		public String getStrValue()
//	{
//		if (m_parameterType == EsnActionParameterType.DEPENDENT)
//		{
//			// TODO: handle changing dependent parameters
//			if (m_dependentParam == null)
//			{
//				// Cache this duder
//				m_dependentParam = ActionManager.getInstance().getParameter(getDependentParamId());
//			}
//
//			if (m_dependentParam != null)
//			{
//				Logger.w(TAG, "getting dependent param value with no dependent param");
//				return m_dependentParam.getStrValue();
//			}
//		}
//		return super.getStrValue();
//	}
//
//	/* (non-Javadoc)
//     * @see synet.controller.actions.IParameter#isEqualTo(synet.controller.actions.IParameter, boolean)
//     */
//	@Override
//		public boolean isEqualTo(IParameter p_param, boolean p_compareId)
//	{
//		if (p_param instanceof ActionParameter)
//		{
//			return isEqualTo((ActionParameter)p_param, p_compareId);
//		}
//		return false;
//	}
//
//	/* (non-Javadoc)
//     * @see synet.controller.actions.IActionParameter#isEqualTo(synet.controller.actions.IActionParameter, boolean)
//     */
//	@Override
//		public boolean isEqualTo(IActionParameter p_param, boolean p_compareId)
//	{
//		if (p_param instanceof ActionParameter)
//		{
//			return isEqualTo((ActionParameter)p_param, p_compareId);
//		}
//		return false;
//	}
//}
//
