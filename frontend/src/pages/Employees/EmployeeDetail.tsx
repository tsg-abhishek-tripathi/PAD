import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Card,
  Row,
  Col,
  Descriptions,
  Tag,
  Button,
  Space,
  Table,
  Typography,
  Divider,
  Spin,
  message,
  Badge,
  Avatar,
  Statistic,
} from 'antd';
import {
  EditOutlined,
  ArrowLeftOutlined,
  UserOutlined,
  MailOutlined,
  BankOutlined,
  CalendarOutlined,
  TeamOutlined,
  TrophyOutlined,
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { AppDispatch, RootState } from '../../store/store';
import {
  fetchEmployeeDetail,
  clearSelectedEmployee,
  selectSelectedEmployee,
  selectIsLoadingDetail,
  selectError,
  Employee,
  EmployeeAffiliation,
} from '../../store/slices/employeeSlice';

const { Title, Text } = Typography;

const EmployeeDetail: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();
  const { employeeId } = useParams<{ employeeId: string }>();
  
  const selectedEmployee = useSelector(selectSelectedEmployee);
  const isLoadingDetail = useSelector(selectIsLoadingDetail);
  const error = useSelector(selectError);

  useEffect(() => {
    if (employeeId) {
      dispatch(fetchEmployeeDetail(parseInt(employeeId)));
    }
    
    return () => {
      dispatch(clearSelectedEmployee());
    };
  }, [dispatch, employeeId]);

  useEffect(() => {
    if (error) {
      message.error(error);
    }
  }, [error]);

  const handleEdit = () => {
    navigate(`/employees/${employeeId}/edit`);
  };

  const handleBack = () => {
    navigate('/employees');
  };

  const affiliationColumns: ColumnsType<EmployeeAffiliation> = [
    {
      title: 'Role Type',
      dataIndex: 'roleType',
      key: 'roleType',
      width: 150,
      render: (roleType) => (
        <Tag color="blue">{roleType}</Tag>
      ),
    },
    {
      title: 'Practice',
      dataIndex: 'practice',
      key: 'practice',
      width: 150,
      render: (practice) => (
        <Tag color="green">{practice}</Tag>
      ),
    },
    {
      title: 'Location Scope',
      dataIndex: 'locationScope',
      key: 'locationScope',
      width: 150,
      render: (locationScope) => locationScope || '-',
    },
    {
      title: 'Effective Date',
      dataIndex: 'effectiveDate',
      key: 'effectiveDate',
      width: 120,
      render: (date) => new Date(date).toLocaleDateString(),
    },
    {
      title: 'Status',
      dataIndex: 'isActive',
      key: 'isActive',
      width: 100,
      render: (isActive) => (
        <Tag color={isActive ? 'success' : 'error'}>
          {isActive ? 'Active' : 'Inactive'}
        </Tag>
      ),
    },
  ];

  if (isLoadingDetail) {
    return (
      <div style={{ padding: '24px', textAlign: 'center' }}>
        <Spin size="large" />
        <div style={{ marginTop: 16 }}>Loading employee details...</div>
      </div>
    );
  }

  if (!selectedEmployee) {
    return (
      <div style={{ padding: '24px', textAlign: 'center' }}>
        <Title level={4}>Employee not found</Title>
        <Button onClick={handleBack}>Back to Employees</Button>
      </div>
    );
  }

  return (
    <div style={{ padding: '24px' }}>
      {/* Header */}
      <Card style={{ marginBottom: 24 }}>
        <Row gutter={[16, 16]} align="middle">
          <Col>
            <Avatar 
              size={64} 
              icon={<UserOutlined />}
              src={`https://ui-avatars.com/api/?name=${selectedEmployee.fullName}&size=64&background=1890ff&color=fff`}
            />
          </Col>
          <Col flex="auto">
            <Title level={2} style={{ margin: 0 }}>
              {selectedEmployee.fullName}
            </Title>
            <Text type="secondary">
              {selectedEmployee.employeeCode} • {selectedEmployee.email}
            </Text>
          </Col>
          <Col>
            <Space>
              <Button 
                icon={<ArrowLeftOutlined />} 
                onClick={handleBack}
              >
                Back
              </Button>
              <Button 
                type="primary" 
                icon={<EditOutlined />} 
                onClick={handleEdit}
              >
                Edit
              </Button>
            </Space>
          </Col>
        </Row>
      </Card>

      <Row gutter={24}>
        {/* Employee Information */}
        <Col span={16}>
          <Card title="Employee Information" style={{ marginBottom: 24 }}>
            <Descriptions column={2} bordered>
              <Descriptions.Item label="Employee Code" span={1}>
                <Text strong>{selectedEmployee.employeeCode}</Text>
              </Descriptions.Item>
              <Descriptions.Item label="Full Name" span={1}>
                {selectedEmployee.fullName}
              </Descriptions.Item>
              <Descriptions.Item label="Email" span={2}>
                <Space>
                  <MailOutlined />
                  {selectedEmployee.email}
                </Space>
              </Descriptions.Item>
              <Descriptions.Item label="Job Title" span={1}>
                {selectedEmployee.title || '-'}
              </Descriptions.Item>
              <Descriptions.Item label="Level" span={1}>
                <Tag color={selectedEmployee.level === 'Partner' ? 'gold' : 'blue'}>
                  {selectedEmployee.level}
                </Tag>
              </Descriptions.Item>
              <Descriptions.Item label="Home Office" span={1}>
                <Space>
                  <BankOutlined />
                  {selectedEmployee.homeOffice?.officeName || '-'}
                </Space>
              </Descriptions.Item>
              <Descriptions.Item label="Region" span={1}>
                {selectedEmployee.homeOffice?.region || '-'}
              </Descriptions.Item>
              <Descriptions.Item label="Status" span={1}>
                <Badge 
                  status={selectedEmployee.isActive ? 'success' : 'error'} 
                  text={selectedEmployee.isActive ? 'Active' : 'Inactive'}
                />
              </Descriptions.Item>
              <Descriptions.Item label="Hire Date" span={1}>
                <Space>
                  <CalendarOutlined />
                  {selectedEmployee.hireDate ? new Date(selectedEmployee.hireDate).toLocaleDateString() : '-'}
                </Space>
              </Descriptions.Item>
              {selectedEmployee.terminationDate && (
                <Descriptions.Item label="Termination Date" span={2}>
                  <Space>
                    <CalendarOutlined />
                    {new Date(selectedEmployee.terminationDate).toLocaleDateString()}
                  </Space>
                </Descriptions.Item>
              )}
            </Descriptions>
          </Card>

          {/* Affiliations */}
          <Card 
            title={
              <Space>
                <TeamOutlined />
                Affiliations ({selectedEmployee.affiliations?.length || 0})
              </Space>
            }
          >
            {selectedEmployee.affiliations && selectedEmployee.affiliations.length > 0 ? (
              <Table
                columns={affiliationColumns}
                dataSource={selectedEmployee.affiliations}
                rowKey="affiliationId"
                pagination={false}
                size="small"
              />
            ) : (
              <div style={{ textAlign: 'center', padding: '40px' }}>
                <Text type="secondary">No affiliations found</Text>
              </div>
            )}
          </Card>
        </Col>

        {/* Statistics and Quick Actions */}
        <Col span={8}>
          <Card title="Statistics" style={{ marginBottom: 24 }}>
            <Row gutter={[16, 16]}>
              <Col span={12}>
                <Statistic
                  title="Affiliations"
                  value={selectedEmployee.affiliations?.length || 0}
                  prefix={<TeamOutlined />}
                  valueStyle={{ color: '#1890ff' }}
                />
              </Col>
              <Col span={12}>
                <Statistic
                  title="Roles"
                  value={selectedEmployee.roles?.length || 0}
                  prefix={<TrophyOutlined />}
                  valueStyle={{ color: '#52c41a' }}
                />
              </Col>
            </Row>
          </Card>

          <Card title="Quick Actions">
            <Space direction="vertical" style={{ width: '100%' }}>
              <Button 
                type="primary" 
                block 
                icon={<EditOutlined />}
                onClick={handleEdit}
              >
                Edit Employee
              </Button>
              <Button 
                block 
                icon={<TeamOutlined />}
                onClick={() => navigate(`/employees/${employeeId}/affiliations`)}
              >
                Manage Affiliations
              </Button>
              <Button 
                block 
                icon={<TrophyOutlined />}
                onClick={() => navigate(`/employees/${employeeId}/roles`)}
              >
                Manage Roles
              </Button>
            </Space>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default EmployeeDetail; 