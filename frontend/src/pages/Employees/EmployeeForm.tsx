import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Form,
  Input,
  Button,
  Card,
  Row,
  Col,
  Select,
  DatePicker,
  Switch,
  message,
  Space,
  Divider,
  Typography,
  Spin,
  Alert,
} from 'antd';
import {
  SaveOutlined,
  CloseOutlined,
  UserOutlined,
  MailOutlined,
  BankOutlined,
  CalendarOutlined,
} from '@ant-design/icons';
import { AppDispatch, RootState } from '../../store/store';
import {
  createEmployee,
  updateEmployee,
  fetchEmployeeDetail,
  clearSelectedEmployee,
  selectSelectedEmployee,
  selectIsLoadingDetail,
  selectError,
} from '../../store/slices/employeeSlice';
import { officeAPI } from '../../services/api';

const { Title, Text } = Typography;
const { Option } = Select;
const { TextArea } = Input;

interface Office {
  officeId: number;
  officeName: string;
  region: string;
}

const EmployeeForm: React.FC = () => {
  const [form] = Form.useForm();
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();
  const { employeeId } = useParams<{ employeeId: string }>();
  
  const selectedEmployee = useSelector(selectSelectedEmployee);
  const isLoadingDetail = useSelector(selectIsLoadingDetail);
  const error = useSelector(selectError);
  
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [offices, setOffices] = useState<Office[]>([]);
  const [isLoadingOffices, setIsLoadingOffices] = useState(false);
  const isEditMode = Boolean(employeeId);

  useEffect(() => {
    loadOffices();
    
    if (isEditMode && employeeId) {
      dispatch(fetchEmployeeDetail(parseInt(employeeId)));
    }
    
    return () => {
      if (isEditMode) {
        dispatch(clearSelectedEmployee());
      }
    };
  }, [dispatch, employeeId, isEditMode]);

  useEffect(() => {
    if (selectedEmployee && isEditMode) {
      form.setFieldsValue({
        employeeCode: selectedEmployee.employeeCode,
        fullName: selectedEmployee.fullName,
        email: selectedEmployee.email,
        title: selectedEmployee.title,
        level: selectedEmployee.level,
        homeOfficeId: selectedEmployee.homeOffice?.officeId,
        isActive: selectedEmployee.isActive,
        hireDate: selectedEmployee.hireDate ? new Date(selectedEmployee.hireDate) : undefined,
        terminationDate: selectedEmployee.terminationDate ? new Date(selectedEmployee.terminationDate) : undefined,
      });
    }
  }, [selectedEmployee, form, isEditMode]);

  useEffect(() => {
    if (error) {
      message.error(error);
    }
  }, [error]);

  const loadOffices = async () => {
    setIsLoadingOffices(true);
    try {
      const response = await officeAPI.getAll();
      setOffices(response.data);
    } catch (error) {
      message.error('Failed to load offices');
    } finally {
      setIsLoadingOffices(false);
    }
  };

  const handleSubmit = async (values: any) => {
    setIsSubmitting(true);
    try {
      const employeeData = {
        ...values,
        hireDate: values.hireDate?.toISOString(),
        terminationDate: values.terminationDate?.toISOString(),
      };

      if (isEditMode && employeeId) {
        await dispatch(updateEmployee({ id: parseInt(employeeId), data: employeeData })).unwrap();
        message.success('Employee updated successfully');
      } else {
        await dispatch(createEmployee(employeeData)).unwrap();
        message.success('Employee created successfully');
      }
      
      navigate('/employees');
    } catch (error) {
      message.error('Failed to save employee');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCancel = () => {
    navigate('/employees');
  };

  if (isLoadingDetail) {
    return (
      <div style={{ padding: '24px', textAlign: 'center' }}>
        <Spin size="large" />
        <div style={{ marginTop: 16 }}>Loading employee details...</div>
      </div>
    );
  }

  return (
    <div style={{ padding: '24px' }}>
      <Card>
        <div style={{ marginBottom: 24 }}>
          <Title level={3}>
            {isEditMode ? 'Edit Employee' : 'Add New Employee'}
          </Title>
          <Text type="secondary">
            {isEditMode 
              ? 'Update employee information and affiliations'
              : 'Create a new employee record with basic information'
            }
          </Text>
        </div>

        <Form
          form={form}
          layout="vertical"
          onFinish={handleSubmit}
          initialValues={{
            isActive: true,
            level: 'Associate',
          }}
        >
          <Row gutter={24}>
            <Col span={12}>
              <Form.Item
                name="employeeCode"
                label="Employee Code"
                rules={[
                  { required: true, message: 'Please enter employee code' },
                  { min: 3, message: 'Employee code must be at least 3 characters' },
                ]}
              >
                <Input 
                  prefix={<UserOutlined />} 
                  placeholder="e.g., EMP001"
                  disabled={isEditMode}
                />
              </Form.Item>
            </Col>
            
            <Col span={12}>
              <Form.Item
                name="fullName"
                label="Full Name"
                rules={[
                  { required: true, message: 'Please enter full name' },
                  { min: 2, message: 'Name must be at least 2 characters' },
                ]}
              >
                <Input 
                  prefix={<UserOutlined />} 
                  placeholder="e.g., John Doe"
                />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={24}>
            <Col span={12}>
              <Form.Item
                name="email"
                label="Email Address"
                rules={[
                  { required: true, message: 'Please enter email address' },
                  { type: 'email', message: 'Please enter a valid email address' },
                ]}
              >
                <Input 
                  prefix={<MailOutlined />} 
                  placeholder="e.g., john.doe@company.com"
                />
              </Form.Item>
            </Col>
            
            <Col span={12}>
              <Form.Item
                name="title"
                label="Job Title"
                rules={[
                  { required: true, message: 'Please enter job title' },
                ]}
              >
                <Input 
                  placeholder="e.g., Senior Associate"
                />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={24}>
            <Col span={8}>
              <Form.Item
                name="level"
                label="Level"
                rules={[
                  { required: true, message: 'Please select level' },
                ]}
              >
                <Select placeholder="Select level">
                  <Option value="Partner">Partner</Option>
                  <Option value="Principal">Principal</Option>
                  <Option value="Senior Manager">Senior Manager</Option>
                  <Option value="Manager">Manager</Option>
                  <Option value="Senior Associate">Senior Associate</Option>
                  <Option value="Associate">Associate</Option>
                  <Option value="Staff">Staff</Option>
                </Select>
              </Form.Item>
            </Col>
            
            <Col span={8}>
              <Form.Item
                name="homeOfficeId"
                label="Home Office"
                rules={[
                  { required: true, message: 'Please select home office' },
                ]}
              >
                <Select 
                  placeholder="Select office"
                  loading={isLoadingOffices}
                  showSearch
                  optionFilterProp="children"
                >
                  {offices.map(office => (
                    <Option key={office.officeId} value={office.officeId}>
                      {office.officeName} ({office.region})
                    </Option>
                  ))}
                </Select>
              </Form.Item>
            </Col>
            
            <Col span={8}>
              <Form.Item
                name="isActive"
                label="Status"
                valuePropName="checked"
              >
                <Switch 
                  checkedChildren="Active" 
                  unCheckedChildren="Inactive"
                />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={24}>
            <Col span={12}>
              <Form.Item
                name="hireDate"
                label="Hire Date"
                rules={[
                  { required: true, message: 'Please select hire date' },
                ]}
              >
                <DatePicker 
                  style={{ width: '100%' }}
                  placeholder="Select hire date"
                />
              </Form.Item>
            </Col>
            
            <Col span={12}>
              <Form.Item
                name="terminationDate"
                label="Termination Date"
              >
                <DatePicker 
                  style={{ width: '100%' }}
                  placeholder="Select termination date (optional)"
                />
              </Form.Item>
            </Col>
          </Row>

          <Divider />

          <Form.Item>
            <Space>
              <Button
                type="primary"
                htmlType="submit"
                icon={<SaveOutlined />}
                loading={isSubmitting}
                size="large"
              >
                {isEditMode ? 'Update Employee' : 'Create Employee'}
              </Button>
              <Button
                icon={<CloseOutlined />}
                onClick={handleCancel}
                size="large"
              >
                Cancel
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Card>
    </div>
  );
};

export default EmployeeForm; 