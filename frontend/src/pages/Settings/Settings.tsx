import React from 'react';
import { Card, Typography, Form, Switch, Button, Space } from 'antd';

const { Title } = Typography;

const Settings: React.FC = () => {
  return (
    <div>
      <Title level={2}>Settings</Title>
      
      <Card title="System Settings">
        <Form layout="vertical">
          <Form.Item label="Enable Notifications" name="notifications">
            <Switch defaultChecked />
          </Form.Item>
          <Form.Item label="Auto-save Forms" name="autoSave">
            <Switch defaultChecked />
          </Form.Item>
          <Form.Item label="Dark Mode" name="darkMode">
            <Switch />
          </Form.Item>
          <Space>
            <Button type="primary">Save Settings</Button>
            <Button>Reset to Default</Button>
          </Space>
        </Form>
      </Card>
    </div>
  );
};

export default Settings; 