import React from 'react';

export const Context = React.createContext();

export default class ContextProvider extends React.Component {
    state = {
        diagramName: "",
        running: false
    }

    render() {
        return (
            <Context.Provider value={
                {
                    state: this.state,
                    setDiagramName: (value) => this.setState({
                        diagramName: value
                    }),
                    setRunning: (value) => this.setState({
                        running: value
                    })
                }
            }>
                {this.props.children}
            </Context.Provider>
        )
    }
}