import React from 'react';

export const Context = React.createContext();

export default class ContextProvider extends React.Component {
    state = {
        diagramName: "",
        running: false,
        paused: false,
        disableStop: true
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
                    }),
                    setPaused: (value) => this.setState({
                        paused: value
                    })
                    ,
                    setDisableStop: (value) => this.setState({
                        disableStop: value
                    })
                }
            }>
                {this.props.children}
            </Context.Provider>
        )
    }
}